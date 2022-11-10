using PX.Data;
using System.Collections;

namespace PX.Objects.AR
{
    public class ARInvoiceEntry_ReleaseExt: PXGraphExtension<ARInvoiceEntry>
    {
        #region Override
        #region ReleaseFromHold
        public delegate IEnumerable ReleaseFromHoldDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable ReleaseFromHold(PXAdapter adapter, ReleaseFromHoldDelegate baseMethod)
        {
            var _return = baseMethod(adapter);
            Base.Persist();
            return _return;
        }
        #endregion
        #endregion
    }
}
