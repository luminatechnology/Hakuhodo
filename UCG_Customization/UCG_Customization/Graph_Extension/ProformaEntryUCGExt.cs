using System;
using PX.Data;

namespace PX.Objects.PM
{
    public class ProformaEntryUCGExt : PXGraphExtension<ProformaEntry>
    {
        #region Override
        public delegate void ValidatePrecedingBeforeReleaseDelegate(PMProforma doc);
        [PXOverride]
        public virtual void ValidatePrecedingBeforeRelease(PMProforma doc, ValidatePrecedingBeforeReleaseDelegate baseMethod)
        {
        }

        public delegate void ValidatePrecedingInvoicesBeforeReleaseDelegate(PMProforma doc);
        [PXOverride]
        public virtual void ValidatePrecedingInvoicesBeforeRelease(PMProforma doc, ValidatePrecedingInvoicesBeforeReleaseDelegate baseMethod)
        {
        }
        #endregion
    }
}
