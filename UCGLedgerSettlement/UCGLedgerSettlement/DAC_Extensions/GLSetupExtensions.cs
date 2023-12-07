using PX.Data;

namespace PX.Objects.GL
{
    public class GLSetupExt : PXCacheExtension<PX.Objects.GL.GLSetup>
    {
        #region UsrChkReferenceOnMatch
        [PXDBBool]
        [PXUIField(DisplayName = "Check Reference On Matching")]
        public virtual bool? UsrChkReferenceOnMatch { get; set; }
        public abstract class usrChkReferenceOnMatch : PX.Data.BQL.BqlBool.Field<usrChkReferenceOnMatch> { }
        #endregion

        #region UsrChkProjectOnMatch
        [PXDBBool]
        [PXUIField(DisplayName = "Check Project On Matching")]
        public virtual bool? UsrChkProjectOnMatch { get; set; }
        public abstract class usrChkProjectOnMatch : PX.Data.BQL.BqlBool.Field<usrChkProjectOnMatch> { }
        #endregion
    }
}
