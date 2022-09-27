using PX.Data;

namespace PX.Objects.AP
{
    public class APRegisterUCGExt : PXCacheExtension<PX.Objects.AP.APRegister>
    {
        #region UsrEPLentAP 
        [PXDBString(15)]
        [PXUIField(DisplayName = "UsrEPLentAP")]
        [PXDBDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string UsrEPLentAP { get; set; }
        public abstract class usrEPLentAP : PX.Data.BQL.BqlString.Field<usrEPLentAP> { }
        #endregion

    }
}