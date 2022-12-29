using System.Linq;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;

namespace PX.Objects.EP
{
    public class ExpenseClaimEntry_Extension : PXGraphExtension<ExpenseClaimEntry>
    {
        #region Selects
        [PXCopyPasteHiddenFields(typeof(TWNManualGUIExpense.gUINbr), typeof(TWNManualGUIExpense.refNbr))]
        public SelectFrom<TWNManualGUIExpense>
                         .Where<TWNManualGUIExpense.refNbr.IsEqual<EPExpenseClaim.refNbr.FromCurrent>>
                         .OrderBy<TWNManualGUIExpense.createdDateTime.Asc>.View manGUIExpense;
        #endregion

        #region Override Method
        public override void Initialize()
        {
            base.Initialize();

            PXCache<AP.Vendor> vendor = new PXCache<AP.Vendor>(Base);

            Base.Caches[typeof(AP.Vendor)] = vendor;

            PXCache<EPEmployee> employee = new PXCache<EPEmployee>(Base);

            Base.Caches[typeof(EPEmployee)] = employee;
        }
        #endregion

        #region Static Methods
        public static bool IsActive() => TWNGUIValidation.ActivateTWGUI(new PXGraph());
        #endregion

        #region Cache Attached
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [GL.Branch(typeof(AccessInfo.branchID))]
        protected virtual void _(Events.CacheAttached<EPExpenseClaim.branchID> e) { }
        #endregion

        #region Event Handlers

        #region EPExpenseClaim 
        TWNGUIValidation tWNGUIValidation = new TWNGUIValidation();

        protected void _(Events.RowSelected<EPExpenseClaim> e, PXRowSelected InvokeBaseHandler)
        {
            InvokeBaseHandler?.Invoke(e.Cache, e.Args);

            manGUIExpense.Cache.AllowSelect = TWNGUIValidation.ActivateTWGUI(Base);
            manGUIExpense.Cache.AllowDelete = manGUIExpense.Cache.AllowInsert = manGUIExpense.Cache.AllowUpdate = !e.Row.Status.Equals(EPExpenseClaimStatus.ReleasedStatus);
        }

        protected void _(Events.RowPersisting<EPExpenseClaim> e, PXRowPersisting baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            if (e.Row != null)
            {
                decimal taxSum = 0;

                foreach (TWNManualGUIExpense row in manGUIExpense.Select())
                {
                    tWNGUIValidation.CheckCorrespondingInv(Base, row.GUINbr, row.VATInCode);

                    if (tWNGUIValidation.errorOccurred.Equals(true))
                    {
                        manGUIExpense.Cache.RaiseExceptionHandling<TWNManualGUIExpense.gUINbr>(row, row.GUINbr, new PXSetPropertyException(tWNGUIValidation.errorMessage, PXErrorLevel.RowError));
                    }

                    taxSum += row.TaxAmt.Value;
                }

                if (taxSum != 0m && taxSum != e.Row.TaxTotal && e.Row.Hold == false)
                {
                    var current = manGUIExpense.Current ?? manGUIExpense.Cache.Cached.RowCast<TWNManualGUIExpense>().FirstOrDefault();

                    manGUIExpense.Cache.RaiseExceptionHandling<TWNManualGUIExpense.taxAmt>(current, current?.TaxAmt, new PXSetPropertyException(TWMessages.ChkTotalGUIAmt));
                    
                    throw new PXException(Common.Messages.RecordCanNotBeSaved);
                }
            }
        }
        #endregion

        #region TWNManualGUIExpense
        protected virtual void _(Events.RowInserting<TWNManualGUIExpense> e)
        {
            e.Row.SortOrder = e.Cache.Cached.RowCast<TWNManualGUIExpense>().Count() + 1;
        }

        protected virtual void _(Events.FieldDefaulting<TWNManualGUIExpense.deduction> e)
        {
            var row = (TWNManualGUIExpense)e.Row;

            /// If user doesn't choose a vendor then bring the fixed default value from Attribure "DEDUCTCODE" first record.
            e.NewValue = row.VendorID == null ? "1" : e.NewValue;
        }

        protected virtual void _(Events.FieldDefaulting<TWNManualGUIExpense.branchID> e)
        {
            e.NewValue = Base.ExpenseClaim.Current?.BranchID;

            // Since the BranchAttribute will bring default value, it cannot immediately respond to the new value to the event and trigger the related event.
            manGUIExpense.Cache.SetValueExt<TWNManualGUIExpense.ourTaxNbr>(e.Row, BAccountExt.GetOurTaxNbByBranch(e.Cache, (int?)e.NewValue));
        }

        protected virtual void _(Events.FieldDefaulting<TWNManualGUIExpense.createdDateTime> e)
        {
            e.NewValue = System.DateTime.UtcNow;
        }
        #endregion

        #endregion
    }
}