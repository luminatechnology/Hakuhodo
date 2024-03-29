﻿using PX.Data;
using PX.Objects.PO;
using PX.Objects.GL;
using UCG_Customization.Utils;
using UCG_Customization.Descriptor;
using PX.Data.Update;
using PX.Objects.PM;
using PMX = PX.Objects.PM;
using PX.Objects.CT;
using PX.Objects.EP;

namespace PX.Objects.CN.Subcontracts.SC.Graphs
{
    public class SubcontractEntryUCGExt : PXGraphExtension<SubcontractEntry>
    {

        #region Event
        #region POOrder
        protected virtual void _(Events.RowSelected<POOrder> e)
        {
            if (e.Row == null) return;
            if (e.Cache.GetStatus(e.Row) != PXEntryStatus.Updated && e.Row.GetExtension<POOrderUCGExt>()?.IsApproving == true && Base.Accessinfo.ScreenID == "SC.30.10.00")
            {
                // Acuminator disable once PX1073 ExceptionsInRowPersisted [Justification]
                // Acuminator disable once PX1045 PXGraphCreateInstanceInEventHandlers [Justification]
                throw new PXRedirectRequiredException(PXGraph.CreateInstance<EPApprovalProcess>(), "EPApprovalProcess");
            }
        }

        protected virtual void _(Events.FieldUpdated<POOrder, POOrder.branchID> e)
        {
            if (e.Row == null) return;
            e.Cache.SetValueExt<POOrder.projectID>(e.Row, null);
        }
        #endregion


        #region POLine
        protected void _(Events.RowInserted<POLine> e, PXRowInserted baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            var row = e.Row;
            if (row == null) return;
            //[Action AddProjectItem] set TranDesc to PMCostBudget.Description
            setDesc(ref row);
        }

        protected virtual void _(Events.RowPersisted<POLine> e)
        {
            if (e.Row == null) return;
            e.Cache.SetDefaultExt<POLineUCGExt.usedExpense>(e.Row);
            e.Cache.SetDefaultExt<POLineUCGExt.budgetAmt>(e.Row);
        }

        protected virtual void _(Events.RowSelected<POLine> e)
        {
            if (e.Row == null) return;
            ValidateUsedExpense(e.Row);
        }

        protected virtual void _(Events.FieldDefaulting<POLine, POLineUCGExt.usedExpense> e)
        {
            if (e.Row == null) return;
            Account account = Account.PK.Find(Base, e.Row.ExpenseAcctID);
            e.NewValue = SqlFunction.P_UsedExpense(
                   AmountType.USED_EXPENSE,
                   PXInstanceHelper.CurrentCompany,
                   e.Row.ProjectID,
                   e.Row.TaskID,
                   e.Row.InventoryID,
                   account?.AccountGroupID);
        }
        protected virtual void _(Events.FieldDefaulting<POLine, POLineUCGExt.budgetAmt> e)
        {
            if (e.Row == null) return;
            Account account = Account.PK.Find(Base, e.Row.ExpenseAcctID);
            e.NewValue = SqlFunction.P_UsedExpense(
                   AmountType.BUDGET_EXPENSE,
                   PXInstanceHelper.CurrentCompany,
                   e.Row.ProjectID,
                   e.Row.TaskID,
                   e.Row.InventoryID,
                   account?.AccountGroupID);
        }

        #endregion
        #endregion

        #region CacheAttached
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<POOrder.branchID>>, Or<PMProject.defaultBranchID, IsNull>>), "專案所屬分公司與分公司不相符")]
        protected virtual void _(Events.CacheAttached<POOrder.projectID> e) { }
        #endregion

        #region Method
        private void setDesc(ref POLine row)
        {
            Account account = Account.PK.Find(Base, row.ExpenseAcctID);
            PMCostBudget costBudget = PMCostBudget.PK.Find(Base, row.ProjectID, row.TaskID, account?.AccountGroupID, row.CostCodeID, row.InventoryID);
            if (costBudget != null) row.TranDesc = costBudget.Description;
        }

        protected virtual void ValidateUsedExpense(POLine item)
        {
            if (item == null) return;
            var rowExt = item.GetExtension<POLineUCGExt>();
            if (rowExt.UsedExpense > rowExt.BudgetAmt)
                ErrorMsg.SetError<POLineUCGExt.usedExpense>(Base.Transactions.Cache, item, rowExt.UsedExpense, "超出預算", PXErrorLevel.Warning);
        }
        #endregion
    }
}
