using System;
using PX.Data;

namespace PX.Objects.PO
{
    public class POOrderEntryUCGExt : PXGraphExtension<POOrderEntry>
    {
        #region Override
        public delegate bool NeedsAPInvoiceDelegate();
        [PXOverride]
        public virtual bool NeedsAPInvoice(NeedsAPInvoiceDelegate baseMethod)
        {
            return true;
        }
        #endregion
    }
}
