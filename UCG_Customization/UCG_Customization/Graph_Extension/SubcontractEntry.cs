
using PX.Data;
using PX.Objects.GL;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.EP;
using UCG_Customization.Utils;

namespace PX.Objects.CN.Subcontracts.SC.Graphs
{
    public class SubcontractEntry_WorkGroupExtension : PXGraphExtension<PX.Objects.CN.Subcontracts.SC.Graphs.SubcontractEntry>
    {
        #region Event

        protected void _(Events.RowInserting<POOrder> e, PXRowInserting baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            var row = e.Row;
            if (row == null) return;
            // Acuminator disable once PX1045 PXGraphCreateInstanceInEventHandlers [Justification]
            SetUsrApproveWG(e.Cache, row);
        }

        protected void _(Events.FieldUpdated<POOrder,POOrder.employeeID> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            if (e.Row == null) return;
            // Acuminator disable once PX1045 PXGraphCreateInstanceInEventHandlers [Justification]
            SetUsrApproveWG(e.Cache, e.Row);
        }

        protected void _(Events.FieldUpdated<POOrder, POOrder.projectID> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            if (e.Row == null) return;
            // Acuminator disable once PX1045 PXGraphCreateInstanceInEventHandlers [Justification]
            SetUsrApproveWG(e.Cache, e.Row);
        }

        #endregion

        #region Method
        private void SetUsrApproveWG(PXCache cache, POOrder row)
        {
            if (cache.GetStatus(row) == PXEntryStatus.Deleted) return;
            int? projectID = row.ProjectID;
            var emp = EPEmployee.PK.Find(Base, row.EmployeeID);
            ApproveWGUtil.SetUsrApproveWG(cache, row, emp.DepartmentID, emp.AcctCD?.Trim() , projectID);
        }
        #endregion

    }
}