
using PX.Data;
using PX.Objects.GL;
using PX.Objects.PM;
using PX.Objects.PO;

namespace PX.Objects.CN.Subcontracts.SC.Graphs
{
    public class SubcontractEntry_UCGExtension : PXGraphExtension<PX.Objects.CN.Subcontracts.SC.Graphs.SubcontractEntry>
    {
        #region Event

        protected void _(Events.RowInserted<POLine> e, PXRowInserted baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            var row = e.Row;
            if (row == null) return;
            //[Action AddProjectItem] set TranDesc to PMCostBudget.Description
            setDesc(ref row);
        }

        #endregion

        #region Method
        private void setDesc(ref POLine row) {
            Account account = Account.PK.Find(Base, row.ExpenseAcctID);
            PMCostBudget costBudget = PMCostBudget.PK.Find(Base, row.ProjectID, row.TaskID, account?.AccountGroupID, row.CostCodeID, row.InventoryID);
            if (costBudget != null) row.TranDesc = costBudget.Description;
        }
        #endregion
    }
}