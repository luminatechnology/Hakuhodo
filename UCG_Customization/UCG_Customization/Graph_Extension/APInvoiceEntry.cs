using PX.Data;
using PX.Objects.CT;
using PX.Objects.EP;
using PX.Objects.PM;
using PX.TM;
using System.Collections.Generic;
using UCG_Customization.Utils;

namespace PX.Objects.AP
{
    public class APInvoiceEntry_WorkGroupExtension : PXGraphExtension<PX.Objects.AP.APInvoiceEntry>
    {

        #region const
        /**User Defined */
        public const string UD_PROJECT = "AttributePROJECT";
        #endregion

        #region Event
        #region APRegister
        protected void _(Events.RowSelected<APRegister> e, PXRowSelected baseMethod)
        {
            var row = e.Row;
            if (row == null) return;
            Base.releaseFromHold.SetEnabled(true);
            baseMethod?.Invoke(e.Cache, e.Args);
            string docType = row.DocType;
            bool isShowReturnAmt = docType == APDocType.Invoice;
            PXUIFieldAttribute.SetVisible<APRegisterUCGExt.returnAmount>(e.Cache, row, isShowReturnAmt);
            PXUIFieldAttribute.SetVisible<APRegisterUCGExt.usrOpportunityID>(e.Cache, row, ProjectDefaultAttribute.IsNonProject(row.ProjectID));
            PXUIFieldAttribute.SetVisible<APTranUCGExt.returnAmt>(Base.Transactions.Cache, null, isShowReturnAmt);
            if (Base.Accessinfo.ScreenID == "AP.30.10.00" && e.Cache.GetStatus(e.Row) == PXEntryStatus.Inserted) Base.releaseFromHold.SetEnabled(false);
        }

        protected void _(Events.RowPersisting<APRegister> e)
        {
            var row = e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<APRegisterWorkGroupExt.usrDepartmentID>(e.Row);
            // Acuminator disable once PX1045 PXGraphCreateInstanceInEventHandlers [Justification]
            SetUsrApproveWG(e.Cache, row);
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
            EPEmployee emp = EPEmployee.UK.Find(Base, GetApproveEmp(e.Cache, row));
            var newValue = emp.DepartmentID;
            e.NewValue = newValue;
            var rowExt = e.Cache.GetExtension<APRegisterWorkGroupExt>(row);
            rowExt.UsrDepartmentID = newValue;
        }

        protected void _(Events.FieldUpdated<APRegister, APRegister.projectID> e, PXFieldUpdated baseMethod)
        {
            var row = e.Row;
            baseMethod?.Invoke(e.Cache, e.Args);
            if (row == null) return;
            bool isNonProject = ProjectDefaultAttribute.IsNonProject(row.ProjectID);
            if (!isNonProject)
            {
                e.Cache.SetValueExt<APRegisterUCGExt.usrOpportunityID>(row, null);
            }
        }
        #endregion
        #endregion

        #region CacheAttached
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXRestrictor(typeof(Where<Contract.defaultBranchID, Equal<Current<APInvoice.branchID>>, Or<Contract.defaultBranchID, IsNull>>), "專案所屬分公司與分公司不相符")]
        protected virtual void _(Events.CacheAttached<APInvoice.projectID> e) { }
        #endregion

        #region Method
        private void SetUsrApproveWG(PXCache cache, APRegister row)
        {
            int? projectID = row.ProjectID;
            string oppID = row.GetExtension<APRegisterUCGExt>().UsrOpportunityID;
            if (row.Hold == true && cache.GetStatus(row) == PXEntryStatus.Deleted) return;
            if (row.DocType == APDocType.Prepayment)
            {
                var projectCD = (PXStringState)cache.GetValueExt(row, UD_PROJECT);
                var pmProject = GetProjectByCD(projectCD);
                if (pmProject == null) throw new PXException($"找不到對應專案:{projectCD}");
                projectID = pmProject.ContractID;
            }
            ApproveWGUtil.SetUsrApproveWG(
                cache, row,
                row.GetExtension<APRegisterWorkGroupExt>().UsrDepartmentID,
                GetApproveEmp(cache, row), projectID, oppID);
        }

        /// <summary>
        /// 取得判斷審核人員
        /// </summary>
        /// <returns></returns>
        protected virtual string GetApproveEmp(PXCache cache, APRegister row)
        {
            EPEmployee emp = null;
            //Step1. 判斷[更多] 中的 員工代墊/廠商代墊
            string vendor = (PXStringState)cache.GetValueExt(row, APInvoiceEntry_ReleaseExt.VENDOR);
            if (!string.IsNullOrEmpty(vendor))
            {
                emp = EPEmployee.UK.Find(Base, vendor);
                if (emp != null) return vendor.Trim();
            }
            //Step2. 判斷供應商
            emp = EPEmployee.PK.Find(Base, row.VendorID);
            if (emp != null) return emp.AcctCD.Trim();
            //Step3. 判斷經辦人
            emp = GetEmployee(row.EmployeeID);
            return emp?.AcctCD?.Trim();
        }

        #endregion

        #region BQL
        private EPEmployee GetEmployee(int? employeeID)
        {
            //APRegister的employeeID其實是ContactID
            return PXSelect<EPEmployee, Where<EPEmployee.defContactID, Equal<Required<EPEmployee.defContactID>>>>.Select(Base, employeeID);
        }

        private PMProject GetProjectByCD(string projectCD)
        {
            return PXSelect<PMProject, Where<PMProject.contractCD, Equal<Required<PMProject.contractCD>>>>
                .Select(Base, projectCD);
        }

        #endregion

    }
}