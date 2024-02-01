using PX.Common;
using PX.Data;
using PX.Objects.CT;
using PX.Objects.EP;
using UCG_Customization.DAC;
using UCG_Customization.Utils;

namespace PX.Objects.PM
{
    public class ProjectEntryUCGExt : PXGraphExtension<PX.Objects.CN.ProjectAccounting.PM.GraphExtensions.ProjectEntryExt, ProjectEntry>
    {
        #region DisplayName
        [PXLocalizable]
        public class LockProjectBtnName
        {
            public const string LockProject = "Lock Project";
            public const string UnLockProject = "UnLock Project";
        }
        #endregion

        public static bool IsActive()
        {
            return true;
        }

        #region View
        [PXViewName("PM Summary By BIV")]
        public PXSelect<PMSummaryByBIV, Where<PMSummaryByBIV.contractID, Equal<Current<PMProject.contractID>>>> BISummary;
        #endregion

        #region Action
        public PXAction<PMProject> LockBtn;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = LockProjectBtnName.LockProject)]
        public void lockBtn()
        {
            var current = Base.Project.Current;
            SetLockProject(current, true);
        }

        public PXAction<PMProject> UnLockBtn;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = LockProjectBtnName.UnLockProject)]
        public void unLockBtn()
        {
            var current = Base.Project.Current;
            SetLockProject(current, false);
        }
        #endregion

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

        #region Events
        public virtual void _(Events.RowSelected<PMProject> e)
        {
            if (e.Row == null) return;
            bool isLocked = e.Row.GetExtension<ContractUCGExt>()?.UsrProjectisLocked ?? false;
            //Lock & UnLock
            LockBtn.SetEnabled(e.Row.Status == Contract.status.Completed);
            UnLockBtn.SetEnabled(e.Row.Status == Contract.status.Completed);
            LockBtn.SetVisible(!isLocked);
            UnLockBtn.SetVisible(isLocked);
            if (e.Row.Status == Contract.status.Completed)
            {
                //Activate Project
                Base.activate.SetEnabled(!isLocked);
                //Run Project Billing
                Base.bill.SetEnabled(!isLocked);
                //Run Allocation
                Base.runAllocation.SetEnabled(!isLocked);
                //Cost Projection
                Base1.costProjection.SetEnabled(!isLocked);
            }
        }

        public virtual void _(Events.RowPersisting<PMProject> e, PXRowPersisting baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);
            if (e.Row == null) return;
            ValidateApprover(e.Cache, e.Row);
        }
        #endregion

        #region Method
        public virtual void ValidateApprover(PXCache cache, PMProject row)
        {
            if (cache.GetStatus(row) == PXEntryStatus.Deleted) return;
            bool flag = false;
            var approver = new string[] { ApproveWGUtil.UD_APPROVE1, ApproveWGUtil.UD_APPROVE2, ApproveWGUtil.UD_APPROVE3, ApproveWGUtil.UD_APPROVE4, ApproveWGUtil.UD_APPROVE5 };
            foreach (var field in approver)
            {
                string val = (PXStringState)cache.GetValueExt(row, field);
                flag = flag || val != null;
            }
            if (!flag)
                throw new PXRowPersistingException(ApproveWGUtil.UD_APPROVE5, null, $"Approver cannot be empty.");
        }

        public virtual void SetLockProject(PMProject row, bool isLock)
        {
            Base.Project.SetValueExt<ContractUCGExt.usrProjectisLocked>(row, isLock);
            Base.Project.Update(row);
            Base.Persist();
        }
        #endregion
    }

}
