using PX.Data;

namespace PX.Objects.AP
{
    public class APPaymentEntryUCGExt:PXGraphExtension<APPaymentEntry>
    {

        #region CacheAttached

        [PXMergeAttributes(Method =MergeMethod.Append)]
        [PXRemoveBaseAttribute(typeof(PXFormulaAttribute))]
        protected virtual void _(Events.CacheAttached<APPayment.branchID> e) { }
        #endregion
    }
}
