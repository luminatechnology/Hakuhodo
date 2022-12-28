using PX.Data;
using PX.Objects.CT;
using PX.Objects.PM;
using System.Collections;

namespace PX.Objects.EP
{
    public class ExpenseClaimMaintUCGExt : PXGraphExtension<ExpenseClaimMaint>
    {

        public PXAction<EPExpenseClaim> ViewProject;
        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXButton]
        public virtual IEnumerable viewProject(PXAdapter adapter)
        {

            var projectID = Base.Claim.Current.GetExtension<EPExpenseClaimWorkGroupExt>().UsrProjectID;
            if (projectID != null && !ProjectDefaultAttribute.IsNonProject(projectID))
            {
                EntityHelper helper = new EntityHelper(Base);
                PMProject project = PMProject.PK.Find(Base, projectID);
                helper.NavigateToRow(project.NoteID.Value, PXRedirectHelper.WindowMode.NewWindow);
            }
            return adapter.Get();
        }

        #region CacheAttached
        [PXMergeAttributes(Method =MergeMethod.Merge)]
        [PXUIField(DisplayName = "Project Description")]
        protected virtual void _(Events.CacheAttached<Contract.description> e) { }
        #endregion
    }
}
