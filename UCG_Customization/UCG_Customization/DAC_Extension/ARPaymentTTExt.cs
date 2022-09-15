using PX.Data;

namespace PX.Objects.AR
{
    public class ARPaymentTTExt : PXCacheExtension<PX.Objects.AR.ARPayment>
    {

        #region UsrIsTTGenerated 
        [PXDBBool]
        [PXUIField(DisplayName = "UsrIsTTGenerated")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrIsTTGenerated { get; set; }
        public abstract class usrIsTTGenerated : PX.Data.BQL.BqlBool.Field<usrIsTTGenerated> { }
        #endregion
    }
}