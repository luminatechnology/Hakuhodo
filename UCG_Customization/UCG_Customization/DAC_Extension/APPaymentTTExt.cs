using PX.Data;

namespace PX.Objects.AP
{
    public class APPaymentTTExt : PXCacheExtension<PX.Objects.AP.APPayment>
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