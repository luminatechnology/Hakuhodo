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

        #region Event Handlers
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
            //�w�I�ڤ����o���� & �������h
            if (row.DocType == APDocType.Prepayment) return;
            var newValue = GetDepartmentID(row.EmployeeID);
            e.NewValue = newValue;
            var rowExt = e.Cache.GetExtension<APRegisterWorkGroupExt>(row);
            rowExt.UsrDepartmentID = newValue;
            SetUsrApproveWG(rowExt);
        }

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
            //���o���e���h
            EPCompanyTree thisTree = GetCompanyTreeByDeptID(depId);
            //���䤣��𪺮ɭԥ����M�Ŭ�Null
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

            //tempGroupIDs ����A�P�ɨ��o������employeeCD
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
            //ParentID ��0 �N�������̤W�h
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