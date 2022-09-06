using PX.Data;
using PX.Objects.EP;
using PX.TM;
using System.Collections.Generic;

namespace PX.Objects.AP
{
    public class APInvoiceEntry_WorkGroupExtension : PXGraphExtension<PX.Objects.AP.APInvoiceEntry>
    {
        #region Event Handlers
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
            var newValue = GetDepartmentID(row.EmployeeID);
            e.NewValue = newValue;
            var rowExt = e.Cache.GetExtension<APRegisterWorkGroupExt>(row);
            rowExt.UsrDepartmentID = newValue;
            SetUsrApproveWG(rowExt);
        }

        #endregion

        #region Method
        private void SetUsrApproveWG(APRegisterWorkGroupExt rowExt)
        {
            var depId = rowExt.UsrDepartmentID;
            //���o��e���h
            EPCompanyTree thisTree = GetCompanyTreeByDeptID(depId);
            if (thisTree == null) return;
            //�إߵ�c�Ŷ�
            int?[] saveGroupIDs = new int?[10];
            List<int?> tempGroupIDs = new List<int?>();
            tempGroupIDs.Add(thisTree.WorkGroupID);

            LoopGetCompanyTreeByParentID(tempGroupIDs, thisTree?.ParentWGID);

            //tempGroupIDs ����
            int j = 0;
            for (int i = tempGroupIDs.Count - 1; i >= 0; i--)
            {
                saveGroupIDs[j++] = tempGroupIDs[i];
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

        private string GetDepartmentID(int? employeeID)
        {
            //APRegister��employeeID���OContactID
            EPEmployee employee = PXSelect<EPEmployee, Where<EPEmployee.defContactID, Equal<Required<EPEmployee.defContactID>>>>.Select(Base, employeeID);
            return employee?.DepartmentID;
        }
        #endregion
    }
}