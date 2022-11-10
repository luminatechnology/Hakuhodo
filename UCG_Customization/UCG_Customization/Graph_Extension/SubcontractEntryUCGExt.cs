using PX.Data;
using PX.Objects.PO;
using PX.Objects.GL;
using UCG_Customization.Utils;
using UCG_Customization.Descriptor;
using PX.Data.Update;

namespace PX.Objects.CN.Subcontracts.SC.Graphs
{ 
    public class SubcontractEntryUCGExt:PXGraphExtension<SubcontractEntry>
    {
        #region Event
        #region POLine
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
                   account.AccountGroupID);
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
                   account.AccountGroupID);
        }

        #endregion
        #endregion

        #region Method
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
