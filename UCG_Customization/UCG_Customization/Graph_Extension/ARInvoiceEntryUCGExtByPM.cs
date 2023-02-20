using PX.Data;
using System;

namespace PX.Objects.AR
{
    public class ARInvoiceEntryUCGExtByPM : PXGraphExtension<PM.ARInvoiceEntryExt,ARInvoiceEntry>
    {
        protected virtual void _(Events.RowDeleting<ARInvoice> e, PXRowDeleting baseMedhod)
        {
            if (e.Row == null) return;
            try
            {
                baseMedhod?.Invoke(e.Cache, e.Args);
            }
            catch (PXException ex)
            {
                //為了攔截PM.ARInvoiceEntryExt RowDeleting<ARInvoice> error
            }
        }
    }
}
