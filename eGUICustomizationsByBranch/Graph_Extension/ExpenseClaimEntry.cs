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
                         .Where<TWNManualGUIExpense.refNbr.IsEqual<EPExpenseClaim.refNbr.FromCurrent>>.View manGUIExpense;
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

                if (taxSum != 0m && !taxSum.Equals(e.Row.TaxTotal))
                {
                    manGUIExpense.Cache.RaiseExceptionHandling<TWNManualGUIExpense.taxAmt>(manGUIExpense.Current, taxSum, new PXSetPropertyException(TWMessages.ChkTotalGUIAmt));
                }
            }
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
            manGUIExpense.Cache.SetValueExt<TWNManualGUIExpense.ourTaxNbr>(e.Row, BAccountExt.GetOurTaxNbBymBranch(e.Cache, (int?)e.NewValue));
        }
        #endregion
    }
}