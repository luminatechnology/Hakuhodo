using PX.Data;
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
        const string UD_PROJECT = "AttributePROJECT";
        #endregion

        #region Event
        #region APRegister
        protected void _(Events.RowPersisting<APRegister> e)
        {
            var row = e.Row;
            if (row == null) return;
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
            var emp = GetEmployee(row.EmployeeID);
            var newValue = emp.DepartmentID;
            e.NewValue = newValue;
            var rowExt = e.Cache.GetExtension<APRegisterWorkGroupExt>(row);
            rowExt.UsrDepartmentID = newValue;
        }
        #endregion
        #endregion

        #region Method
        private void SetUsrApproveWG(PXCache cache, APRegister row)
        {
            int? projectID = null;
            if (row.Hold == true && cache.GetStatus(row) == PXEntryStatus.Deleted) return;
            if (row.DocType == APDocType.Prepayment) { 
                var projectCD = (PXStringState)cache.GetValueExt(row, UD_PROJECT);
                var pmProject = GetProjectByCD(projectCD);
                projectID = pmProject.ContractID;
            }
            var emp = GetEmployee(row.EmployeeID);
            ApproveWGUtil.SetUsrApproveWG(cache,row, row.GetExtension<APRegisterWorkGroupExt>().UsrDepartmentID, emp.AcctCD , projectID);
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