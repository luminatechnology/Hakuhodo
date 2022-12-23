using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;

namespace PX.Objects.CA
{
    public class CATranEntry_Extension : PXGraphExtension<CATranEntry>
    {
        #region Select & Setup
        // Retrieves detail records by CAAdj.adjRefNbr of the current master record.
        public SelectFrom<TWNManualGUIBank>
                         .Where<TWNManualGUIBank.adjRefNbr.IsEqual<CAAdj.adjRefNbr.FromCurrent>>.View ManGUIBank;

        public PXSetup<TWNGUIPreferences> GUIPreferences;
        #endregion

        #region Override Method
        public override void Initialize()
        {
            base.Initialize();

            PXCache<AP.Vendor> vendor = new PXCache<AP.Vendor>(Base);

            Base.Caches[typeof(AP.Vendor)] = vendor;

            PXCache<EP.EPEmployee> employee = new PXCache<EP.EPEmployee>(Base);

            Base.Caches[typeof(EP.EPEmployee)] = employee;
        }
        #endregion

        #region Event Handlers
        public bool activateGUI = TWNGUIValidation.ActivateTWGUI(new PXGraph());

        TWNGUIValidation tWNGUIValidation = new TWNGUIValidation();

        protected void _(Events.RowSelected<CAAdj> e, PXRowSelected InvokeBaseHandler)
        {
            InvokeBaseHandler?.Invoke(e.Cache, e.Args);

            ManGUIBank.Cache.AllowSelect = activateGUI;
            ManGUIBank.Cache.AllowDelete = ManGUIBank.Cache.AllowInsert = ManGUIBank.Cache.AllowUpdate = !e.Row.Status.Equals(CATransferStatus.Released);
        }

        protected void _(Events.RowPersisting<CAAdj> e, PXRowPersisting baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            if (e.Row == null || activateGUI.Equals(false)) { return; }

            decimal taxSum = 0m;

            foreach (TWNManualGUIBank row in ManGUIBank.Select())
            {
                tWNGUIValidation.CheckCorrespondingInv(Base, row.GUINbr, row.VATInCode);

                if (tWNGUIValidation.errorOccurred.Equals(true))
                {
                    ManGUIBank.Cache.RaiseExceptionHandling<TWNManualGUIExpense.gUINbr>(row, row.GUINbr, new PXSetPropertyException(tWNGUIValidation.errorMessage, PXErrorLevel.RowError));
                }

                taxSum += row.TaxAmt.Value;
            }

            if (taxSum != 0m && taxSum != e.Row.TaxTotal)
            {
                ManGUIBank.Cache.RaiseExceptionHandling<TWNManualGUIBank.taxAmt>(ManGUIBank.Current, taxSum, new PXSetPropertyException(TWMessages.ChkTotalGUIAmt));
            }
        }

        protected virtual void _(Events.FieldDefaulting<TWNManualGUIBank.deduction> e)
        {
            var row = (TWNManualGUIBank)e.Row;

            /// If user doesn't choose a vendor then bring the fixed default value from Attribure "DEDUCTCODE" first record.
            e.NewValue = row.VendorID == null ? "1" : e.NewValue;
        }

        protected virtual void _(Events.FieldDefaulting<TWNManualGUIBank.branchID> e)
        {
            e.NewValue = Base.CAAdjRecords.Current?.BranchID;

            // Since the BranchAttribute will bring default value, it cannot immediately respond to the new value to the event and trigger the related event.
            ManGUIBank.Cache.SetValueExt<TWNManualGUIBank.ourTaxNbr>(e.Row, BAccountExt.GetOurTaxNbBymBranch(e.Cache, (int?)e.NewValue));
        }
        #endregion
    }
}