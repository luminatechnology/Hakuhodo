using PX.Data;

namespace PX.Objects.AR
{
    public class ARInvoiceEntryUCGExt:PXGraphExtension<ARInvoiceEntry>
    {

        #region Event
        protected virtual void _(Events.RowSelected<ARInvoice> e, PXRowSelected baseMedhod) {
            if (e.Row == null) return;
            baseMedhod?.Invoke(e.Cache, e.Args);
            if (e.Row.ProformaExists == true)
                PXUIFieldAttribute.SetEnabled<ARTran.accountID>(Base.Transactions.Cache, null, true);
        }

        #endregion
    }
}
