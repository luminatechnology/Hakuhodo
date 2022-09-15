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
            //���o��e���h
            EPCompanyTree thisTree = GetCompanyTreeByDeptID(depId);
            var rowExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimWorkGroupExt>(row);
            //��䤣��𪺮ɭԥ����M�Ŭ�Null
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
            //�إߵ�c�Ŷ�
            string[] saveGroupIDs = new string[TREE_SIZE];
            List<int?> tempGroupIDs = new List<int?>();
            tempGroupIDs.Add(thisTree.WorkGroupID);

            LoopGetCompanyTreeByParentID(tempGroupIDs, thisTree?.ParentWGID);

            if (tempGroupIDs.Count > TREE_SIZE)
            {
                string ERR_MSG = "���q��´���c�W�L" + TREE_SIZE + "�h�A���p���t�κ޲z�����@�{��";
                throw new PXException(ERR_MSG);
            }

            //tempGroupIDs ���� �A�P�ɨ��o������employeeCD
            int j = 0;
            for (int i = tempGroupIDs.Count - 1; i >= 0; i--)
            {
                saveGroupIDs[j++] = GetTreeMasterEmpCD(tempGroupIDs[i]);
            }

            //�g�J���
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
         * �j��Method --�`�N�ϥ��קK�L�a�j��!!!
         */
        private void LoopGetCompanyTreeByParentID(List<int?> tempIDs, int? parentID)
        {
            //ParentID ��0 �N�����̤W�h
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