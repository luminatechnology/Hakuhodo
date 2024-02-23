using PX.Data;

namespace PX.Objects.GL
{
    public class GLSetupExt : PXCacheExtension<PX.Objects.GL.GLSetup>
    {
        #region UsrEnblSettlingLedgerCmplProj
        [PXDBBool]
        [PXUIField(DisplayName = "Enable Settling Ledger On Completed Projects")]
        public virtual bool? UsrEnblSettlingLedgerCmplProj { get; set; }
        public abstract class usrEnblSettlingLedgerCmplProj : PX.Data.BQL.BqlBool.Field<usrEnblSettlingLedgerCmplProj> { }
        #endregion
    }
}
