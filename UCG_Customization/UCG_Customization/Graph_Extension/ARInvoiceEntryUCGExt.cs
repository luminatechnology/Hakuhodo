using PX.Data;
using System;

namespace PX.Objects.AR
{
    public class ARInvoiceEntryUCGExt : PXGraphExtension<ARInvoiceEntry>
    {
        #region Message
        public const string TAX_NOT_EQ_5PER_OF_LINE_TOTAL = "AR tax does not equal to 5% of detail total";
        #endregion

        #region Override
        public delegate ARRegister OnBeforeReleaseDelegate(ARRegister doc);
        [PXOverride]
        public virtual ARRegister OnBeforeRelease(ARRegister doc, OnBeforeReleaseDelegate baseMethod)
        {
            ARInvoice invoice = doc as ARInvoice;
            if (invoice != null)
            {
                var invoiceExt = invoice.GetExtension<ARInvoiceUCGExt>();
                var lineTotal = invoice.CuryLineTotal ?? 0m;
                var taxRate = 0.05m;
                var taxAmt = Math.Round(lineTotal * taxRate, 0, MidpointRounding.AwayFromZero);
                if (taxAmt != invoice.CuryTaxTotal)
                {
                    throw new PXException(TAX_NOT_EQ_5PER_OF_LINE_TOTAL);
                }
            }
            return baseMethod(doc);
        }
        #endregion

        #region Event
        protected virtual void _(Events.RowSelected<ARInvoice> e, PXRowSelected baseMedhod)
        {
            if (e.Row == null) return;
            baseMedhod?.Invoke(e.Cache, e.Args);
            if (e.Row.ProformaExists == true)
                PXUIFieldAttribute.SetEnabled<ARTran.accountID>(Base.Transactions.Cache, null, true);
        }
        #endregion
    }
}
