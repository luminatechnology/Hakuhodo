using PX.Data;
using PX.Objects.EP;
using PX.Objects.PM;
using PX.TM;
using System.Collections.Generic;

namespace PX.Objects.AP
{
    public class APInvoiceEntry_WorkGroupExtension : PXGraphExtension<PX.Objects.AP.APInvoiceEntry>
    {

        #region const
        /**User Defined */
        const string UD_PROJECT = "AttributePROJECT";
        /**User Defined */
        const string UD_APPROVE1 = "AttributeAPPROVER1";
        /**User Defined */
        const string UD_APPROVE2 = "AttributeAPPROVER2";
        /**User Defined */
        const string UD_APPROVE3 = "AttributeAPPROVER3";
        /**User Defined */
        const string UD_APPROVE4 = "AttributeAPPROVER4";
        /**User Defined */
        const string UD_APPROVE5 = "AttributeAPPROVER5";
        #endregion

        #region Event
        #region APRegister
        protected void _(Events.RowPersisting<APRegister> e)
        {
            var row = e.Row;
            if (row == null) return;
            SetUsrApproveWGByPPM(e.Cache, row);
        }

        protected void _(Events.FieldUpdated<APRegister.employeeID> e)
        {
            var row = e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<APRegisterWorkGroupExt.usrDepartmentID>(row);
        }

        protected void _(Events.FieldDefaulting<APRegisterWorkGroupExt.usrDepartmentID> e)
        {
            APRegister row = (APRegister)e.Row;
            if (row == null && row.EmployeeID != null) return;
            //預付款不取得部門 & 部門階層
            if (row.DocType == APDocType.Prepayment) return;
            var newValue = GetDepartmentID(row.EmployeeID);
            e.NewValue = newValue;
            var rowExt = e.Cache.GetExtension<APRegisterWorkGroupExt>(row);
            rowExt.UsrDepartmentID = newValue;
            SetUsrApproveWG(rowExt);
        }
        #endregion
        #endregion

        #region Method
        private void SetUsrApproveWGByPPM(PXCache cache, APRegister row)
        {
            if (row.DocType != APDocType.Prepayment) return;
            var rowExt = PXCache<APRegister>.GetExtension<APRegisterWorkGroupExt>(row);
            var projectCD = (PXStringState)cache.GetValueExt(row, UD_PROJECT);
            var pmProject = GetProjectByCD(projectCD);
            //if (pmProject == null) return;
            PXCache<PMProject> projectCache = Base.Caches<PMProject>();

            rowExt.UsrApproveWG01 = null;
            rowExt.UsrApproveWG02 = null;
            rowExt.UsrApproveWG03 = null;
            rowExt.UsrApproveWG04 = null;
            rowExt.UsrApproveWG05 = (PXStringState)projectCache.GetValueExt(pmProject, UD_APPROVE5);
            rowExt.UsrApproveWG06 = (PXStringState)projectCache.GetValueExt(pmProject, UD_APPROVE4);
            rowExt.UsrApproveWG07 = (PXStringState)projectCache.GetValueExt(pmProject, UD_APPROVE3);
            rowExt.UsrApproveWG08 = (PXStringState)projectCache.GetValueExt(pmProject, UD_APPROVE2);
            rowExt.UsrApproveWG09 = (PXStringState)projectCache.GetValueExt(pmProject, UD_APPROVE1);
            rowExt.UsrApproveWG10 = null;
        }

        private void SetUsrApproveWG(APRegisterWorkGroupExt rowExt)
        {
            const int TREE_SIZE = 10;
            var depId = rowExt.UsrDepartmentID;
            //取得當前階層
            EPCompanyTree thisTree = GetCompanyTreeByDeptID(depId);
            //當找不到樹的時候全部清空為Null
            if (thisTree == null)
            {
                rowExt.UsrApproveWG01 = null;
                rowExt.UsrApproveWG02 = null;
                rowExt.UsrApproveWG03 = null;
                rowExt.UsrApproveWG04 = null;
                rowExt.UsrApproveWG05 = null;
                rowExt.UsrApproveWG06 = null;
                rowExt.UsrApproveWG07 = null;
                rowExt.UsrApproveWG08 = null;
                rowExt.UsrApproveWG09 = null;
                rowExt.UsrApproveWG10 = null;
                return;
            }
            //建立虛構空間
            string[] saveGroupIDs = new string[TREE_SIZE];
            List<int?> tempGroupIDs = new List<int?>();
            tempGroupIDs.Add(thisTree.WorkGroupID);

            LoopGetCompanyTreeByParentID(tempGroupIDs, thisTree?.ParentWGID);

            if (tempGroupIDs.Count > TREE_SIZE)
            {
                string ERR_MSG = "公司組織結構超過" + TREE_SIZE + "層，請聯絡系統管理員維護程式";
                throw new PXException(ERR_MSG);
            }

            //tempGroupIDs 倒續，同時取得對應的employeeCD
            int j = 0;
            for (int i = tempGroupIDs.Count - 1; i >= 0; i--)
            {
                saveGroupIDs[j++] = GetTreeMasterEmpCD(tempGroupIDs[i]);
            }

            //寫入欄位
            rowExt.UsrApproveWG01 = saveGroupIDs[0];
            rowExt.UsrApproveWG02 = saveGroupIDs[1];
            rowExt.UsrApproveWG03 = saveGroupIDs[2];
            rowExt.UsrApproveWG04 = saveGroupIDs[3];
            rowExt.UsrApproveWG05 = saveGroupIDs[4];
            rowExt.UsrApproveWG06 = saveGroupIDs[5];
            rowExt.UsrApproveWG07 = saveGroupIDs[6];
            rowExt.UsrApproveWG08 = saveGroupIDs[7];
            rowExt.UsrApproveWG09 = saveGroupIDs[8];
            rowExt.UsrApproveWG10 = saveGroupIDs[9];

        }

        /***
         * 迴圈Method --注意使用避免無窮迴圈!!!
         */
        private void LoopGetCompanyTreeByParentID(List<int?> tempIDs, int? parentID)
        {
            //ParentID 為0 代表此為最上層
            if (parentID == null || parentID == 0) return;
            EPCompanyTree parentTree = GetParentCompanyTree(parentID);
            if (parentTree == null) return;
            tempIDs.Add(parentTree.WorkGroupID);

            LoopGetCompanyTreeByParentID(tempIDs, parentTree.ParentWGID);
        }

        #endregion

        #region BQL
        private EPCompanyTree GetCompanyTreeByDeptID(string depId)
        {
            return PXSelectJoin<EPCompanyTree,
                InnerJoin<EPDepartment, On<EPCompanyTree.workGroupID, Equal<EPDepartmentExt.usrWorkGroupID>>>,
                Where<EPDepartment.departmentID, Equal<Required<EPDepartment.departmentID>>>>.Select(Base, depId);
        }

        private EPCompanyTree GetParentCompanyTree(int? thisWorkGroupID)
        {
            return PXSelect<EPCompanyTree,
                Where<EPCompanyTree.workGroupID, Equal<Required<EPCompanyTree.workGroupID>>>>.Select(Base, thisWorkGroupID);
        }

        private string GetDepartmentID(int? employeeID)
        {
            //APRegister的employeeID其實是ContactID
            EPEmployee employee = PXSelect<EPEmployee, Where<EPEmployee.defContactID, Equal<Required<EPEmployee.defContactID>>>>.Select(Base, employeeID);
            return employee?.DepartmentID;
        }

        private string GetTreeMasterEmpCD(int? workGroupID)
        {
            EPEmployee emp = PXSelectJoin<EPEmployee,
                InnerJoin<EPCompanyTreeMember, On<EPCompanyTreeMember.contactID, Equal<EPEmployee.defContactID>>>,
                Where<EPCompanyTreeMember.workGroupID, Equal<Required<EPCompanyTreeMember.workGroupID>>,
                And<EPCompanyTreeMember.active, Equal<True>>>,
                OrderBy<
                 Desc<EPCompanyTreeMember.isOwner>
                >>.Select(new PXGraph(), workGroupID);
            return emp?.AcctCD;
        }

        private PMProject GetProjectByCD(string projectCD)
        {
            return PXSelect<PMProject, Where<PMProject.contractCD, Equal<Required<PMProject.contractCD>>>>
                .Select(Base, projectCD);
        }

        #endregion

    }
}