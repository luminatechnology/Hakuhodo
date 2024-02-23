using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.GL;

namespace UCGLedgerSettlement.Descriptor
{
    public static class LSSettlementFeaturesHelper
    {
        public static bool SettlementEdition
        {
            get
            {
                return SelectFrom<GLSetup>.View.SelectSingleBound(PXGraph.CreateInstance<PXGraph>(), null)
                                               .TopFirst?.GetExtension<GLSetupExt>().UsrEnblSettlingLedgerCmplProj ?? false;
                       //PXAccess.FeatureInstalled("PX.Objects.CS.FeaturesSet+CommerceIntegration");
            }
        }
    }
}
