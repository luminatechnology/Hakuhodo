using PX.Data;
using PX.Data.Update;
using PX.Objects.GL;
using UCG_Customization.Descriptor;
using UCG_Customization.Utils;

namespace PX.Objects.EP
{
    public class ExpenseClaimEntryUCGExt : PXGraphExtension<ExpenseClaimEntry>
    {
        #region Override
        public delegate void PersistDelegate();
        [PXOverride]
        public virtual void Persist(PersistDelegate baseMethod)
        {
            var now = Base.ExpenseClaim.Current;
            baseMethod();
            if (now == null) return;
            var old = EPExpenseClaim.PK.Find(new PXGraph(), now.RefNbr);
            if (old != null)
            {
                //if (Base.Actions.Contains("CancelCloseToList"))
                //var xx = Base.Actions["CancelCloseToList"];
                bool isApproved = old.Approved != true && now.Approved == true;
                bool isRejected = old.Rejected != true && now.Rejected == true;
                //當Approved || Rejected 時，回到EP503010
                if ((isApproved || isRejected) && Base.Accessinfo.ScreenID == "EP.30.10.00")
                {
                    throw new PXRedirectRequiredException(PXGraph.CreateInstance<EPApprovalProcess>(), "EPApprovalProcess");
                }
            }

        }
        #endregion
        #region Event
        #region EPExpenseClaimDetails
        protected virtual void _(Events.RowPersisted<EPExpenseClaimDetails> e)
        {
            if (e.Row == null) return;
            e.Cache.SetDefaultExt<EPExpenseClaimDetailsUCGExt.usedExpense>(e.Row);
            e.Cache.SetDefaultExt<EPExpenseClaimDetailsUCGExt.budgetAmt>(e.Row);
        }

        protected virtual void _(Events.RowSelected<EPExpenseClaimDetails> e)
        {
            if (e.Row == null) return;
            ValidateUsedExpense(e.Row);
        }

        protected virtual void _(Events.FieldDefaulting<EPExpenseClaimDetails, EPExpenseClaimDetailsUCGExt.usedExpense> e)
        {
            if (e.Row == null) return;
            Account account = Account.PK.Find(Base, e.Row.ExpenseAccountID);
            e.NewValue = SqlFunction.P_UsedExpense(
                   AmountType.USED_EXPENSE,
                   PXInstanceHelper.CurrentCompany,
                   e.Row.ContractID,
                   e.Row.TaskID,
                   e.Row.InventoryID,
                   account.AccountGroupID);
        }
        protected virtual void _(Events.FieldDefaulting<EPExpenseClaimDetails, EPExpenseClaimDetailsUCGExt.budgetAmt> e)
        {
            if (e.Row == null) return;
            Account account = Account.PK.Find(Base, e.Row.ExpenseAccountID);
            e.NewValue = SqlFunction.P_UsedExpense(
                   AmountType.BUDGET_EXPENSE,
                   PXInstanceHelper.CurrentCompany,
                   e.Row.ContractID,
                   e.Row.TaskID,
                   e.Row.InventoryID,
                   account.AccountGroupID);
        }

        #endregion
        #endregion

        #region Method
        protected virtual void ValidateUsedExpense(EPExpenseClaimDetails item)
        {
            if (item == null) return;
            var rowExt = item.GetExtension<EPExpenseClaimDetailsUCGExt>();
            if (rowExt.UsedExpense > rowExt.BudgetAmt)
                ErrorMsg.SetError<EPExpenseClaimDetailsUCGExt.usedExpense>(Base.ExpenseClaimDetails.Cache, item, rowExt.UsedExpense, "超出預算", PXErrorLevel.Warning);
        }
        #endregion
    }
}
