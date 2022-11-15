using System.Collections.Generic;
using PX.Data;
using PX.TM;
using UCG_Customization.Utils;

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
            var emp = EPEmployee.PK.Find(Base, row.EmployeeID);
            // Acuminator disable once PX1045 PXGraphCreateInstanceInEventHandlers [Justification]
            ApproveWGUtil.SetUsrApproveWG(e.Cache, row, row.DepartmentID, emp.AcctCD, row.GetExtension<EPExpenseClaimWorkGroupExt>().UsrProjectID);
        }

        protected void _(Events.FieldUpdated<EPExpenseClaim.departmentID> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            var row = (EPExpenseClaim)e.Row;
            if (row == null) return;
            var emp = EPEmployee.PK.Find(Base, row.EmployeeID);
            // Acuminator disable once PX1045 PXGraphCreateInstanceInEventHandlers [Justification]
            ApproveWGUtil.SetUsrApproveWG(e.Cache, row, row.DepartmentID, emp.AcctCD, row.GetExtension<EPExpenseClaimWorkGroupExt>().UsrProjectID);
        }

        protected void _(Events.FieldUpdated<EPExpenseClaim, EPExpenseClaimWorkGroupExt.usrProjectID> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            var row = e.Row;
            if (row == null) return;
            var emp = EPEmployee.PK.Find(Base, row.EmployeeID);
            // Acuminator disable once PX1045 PXGraphCreateInstanceInEventHandlers [Justification]
            ApproveWGUtil.SetUsrApproveWG(e.Cache, row, row.DepartmentID, emp.AcctCD , row.GetExtension<EPExpenseClaimWorkGroupExt>().UsrProjectID);
        }

        #endregion

        #region Method
        #endregion

        #region BQL
        #endregion
    }
}