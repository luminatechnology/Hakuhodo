using PX.Data;
using PX.Objects.EP;
using UCG_Customization.DAC;

namespace PX.Objects.PM
{
    public class ProjectEntryUCGExt:PXGraphExtension<ProjectEntry>
    {
        public static bool IsActive() {
            return true;
        }

        [PXViewName("PM Summary By BIV")]
        public PXSelect<PMSummaryByBIV,Where<PMSummaryByBIV.contractID,Equal<Current<PMProject.contractID>>>> BISummary;

        #region Override
        public delegate void PersistDelegate();
        [PXOverride]
        public virtual void Persist(PersistDelegate baseMethod)
        {
            var now = Base.Project.Current;
            baseMethod();
            if (now == null) return;
            //當Approved 時，回到EP503010
            if (now.GetExtension<PMProjectUCGExt>()?.IsApproving == true && Base.Accessinfo.ScreenID == "PM.30.10.00")
            {
                throw new PXRedirectRequiredException(PXGraph.CreateInstance<EPApprovalProcess>(), "EPApprovalProcess");
            }
        }
        #endregion
    }

}
