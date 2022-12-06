using System;
using PX.Data;
using System.Collections;
using static PX.Objects.EP.EPApprovalProcess;

namespace PX.Objects.EP
{
    public class EPApprovalProcessUCGExt : PXGraphExtension<EPApprovalProcess>
    {

        public PXAction<EPOwned> EditDetail2;
        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXEditDetailButton]
        public IEnumerable editDetail2(PXAdapter adapter)
        {
            try
            {
                Base.editDetail(adapter);
            }
            catch (PXRedirectRequiredException e)
            {
                e.Mode = PXBaseRedirectException.WindowMode.Same;
                throw e;
            }
            return adapter.Get();
        }

    }
}
