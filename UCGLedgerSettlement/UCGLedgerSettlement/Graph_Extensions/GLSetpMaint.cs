using PX.Data;

namespace PX.Objects.GL
{
    public class GLSetupMaint_Extensions : PXGraphExtension<GLSetupMaint>
    {
        #region Delegate Methods
        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            var orig = (bool)(Base.GLSetupRecord.Cache.GetValueOriginal<GLSetupExt.usrEnblSettlingLedgerCmplProj>(Base.GLSetupRecord.Current) ?? false);
            var curr = (bool)(Base.GLSetupRecord.Cache.GetValue<GLSetupExt.usrEnblSettlingLedgerCmplProj>(Base.GLSetupRecord.Current) ?? false);

            baseMethod();

            if (orig != curr)
            {
                PXGraph.CreateInstance<PX.SM.UpdateMaint>().ResetCachesCommand.Press();
            }
        }
        #endregion
    }
}
