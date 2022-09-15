using System.Collections.Generic;
using PX.Data;
using PX.TM;

namespace PX.Objects.EP
{
    public class ExpenseClaimEntry_WorkGroupExtension : PXGraphExtension<PX.Objects.EP.ExpenseClaimEntry>
    {
        #region Event
        protected void _(Events.RowInserting<EPExpenseClaim> e, PXRowInserting baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            var row = e.Row;
            if (row == null) return;
            SetUsrApproveWG(row);
        }

        protected void _(Events.FieldUpdated<EPExpenseClaim.departmentID> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            var row = (EPExpenseClaim)e.Row;
            if (row == null) return;
            SetUsrApproveWG(row);
        }

        #endregion

        #region Method
        private void SetUsrApproveWG(EPExpenseClaim row)
        {
            const int TREE_SIZE = 10;
            var depId = row.DepartmentID;
            //取得當前階層
            EPCompanyTree thisTree = GetCompanyTreeByDeptID(depId);
            var rowExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimWorkGroupExt>(row);
            //當找不到樹的時候全部清空為Null
            if (thisTree == null) {
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

            //tempGroupIDs 倒續 ，同時取得對應的employeeCD
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

        private string GetTreeMasterEmpCD(int? workGroupID)
        {
            EPEmployee emp =  PXSelectJoin<EPEmployee,
                InnerJoin<EPCompanyTreeMember,On<EPCompanyTreeMember.contactID,Equal<EPEmployee.defContactID>>>,
                Where<EPCompanyTreeMember.workGroupID, Equal<Required<EPCompanyTreeMember.workGroupID>>,
                And<EPCompanyTreeMember.active,Equal<True>>>,
                OrderBy<
                 Desc<EPCompanyTreeMember.isOwner>
                >>.Select(new PXGraph(), workGroupID);
            return emp?.AcctCD;
        }
        #endregion
    }
}