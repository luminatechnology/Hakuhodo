using PX.Data;
namespace PX.Objects.AP
{
    public class APSetupUCGExt:PXCacheExtension<APSetup>
    {

        #region UsrIsCheckAdvVendor
        [PXDBBool]
        [PXUIField(DisplayName = "Check AdvVendor")]
        public virtual bool? UsrIsCheckAdvVendor { get; set; }
        public abstract class usrIsCheckAdvVendor : PX.Data.BQL.BqlBool.Field<usrIsCheckAdvVendor> { }
        #endregion


        #region UsrIsCheckAdvAmt
        [PXDBBool]
        [PXUIField(DisplayName = "Check AdvAmount")]
        public virtual bool? UsrIsCheckAdvAmt { get; set; }
        public abstract class usrIsCheckAdvAmt : PX.Data.BQL.BqlBool.Field<usrIsCheckAdvAmt> { }
        #endregion
    }
}
