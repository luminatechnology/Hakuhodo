using PX.Data;
using PX.Data.Update;
using PX.Objects.GL;
using UCG_Customization.Utils;
using UCG_Customization.Descriptor;

namespace PX.Objects.AP
{
    public class APInvoiceEntryUCGExt : PXGraphExtension<PX.Objects.AP.APInvoiceEntry>
    {
        #region Event
        #region APTran
        protected virtual void _(Events.RowPersisted<APTran> e)
        {
            if (e.Row == null) return;
            e.Cache.SetDefaultExt<APTranUCGExt.usedExpense>(e.Row);
            e.Cache.SetDefaultExt<APTranUCGExt.budgetAmt>(e.Row);
        }

        protected virtual void _(Events.RowSelected<APTran> e)
        {
            if (e.Row == null) return;
            ValidateUsedExpense(e.Row);
        }

        protected virtual void _(Events.FieldDefaulting<APTran, APTranUCGExt.usedExpense> e)
        {
            if (e.Row == null) return;
            Account account = Account.PK.Find(Base, e.Row.AccountID);
            e.NewValue = SqlFunction.P_UsedExpense(
                   AmountType.USED_EXPENSE,
                   PXInstanceHelper.CurrentCompany,
                   e.Row.ProjectID,
                   e.Row.TaskID,
                   e.Row.InventoryID,
                   account?.AccountGroupID);
        }
        protected virtual void _(Events.FieldDefaulting<APTran, APTranUCGExt.budgetAmt> e)
        {
            if (e.Row == null) return;
            Account account = Account.PK.Find(Base, e.Row.AccountID);
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

        #region Method
        protected virtual void ValidateUsedExpense(APTran tran) {
            if (tran == null) return;
            var rowExt = tran.GetExtension<APTranUCGExt>();
            if (rowExt.UsedExpense > rowExt.BudgetAmt)
                ErrorMsg.SetError<APTranUCGExt.usedExpense>(Base.Transactions.Cache, tran, rowExt.UsedExpense, "¶W¥X¹wºâ", PXErrorLevel.Warning);
        }
        #endregion
    }
}