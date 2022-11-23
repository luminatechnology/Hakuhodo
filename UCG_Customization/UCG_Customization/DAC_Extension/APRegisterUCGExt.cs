using PX.Data;
using UCG_Customization.Descriptor;

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


        #region Unbound
        #region Return Amount
        [PXDecimal(0)]
        [PXUIField(DisplayName = "Return Amount",IsReadOnly = true)]
        [APReturnAmount(APReturnAmountAttribute.Type.HEADER)]
        public virtual decimal? ReturnAmount { get; set; }
        public abstract class returnAmount : PX.Data.BQL.BqlDecimal.Field<returnAmount> { }
        #endregion
        #endregion

    }
}