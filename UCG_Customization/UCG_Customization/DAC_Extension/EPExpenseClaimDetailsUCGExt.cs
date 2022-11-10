using PX.Data;

namespace PX.Objects.EP
{
    public class EPExpenseClaimDetailsUCGExt:PXCacheExtension<EPExpenseClaimDetails>
    {
        #region UsedExpense 
        [PXDecimal(0)]
        [PXUIField(DisplayName = "Used Expense", IsReadOnly = true)]
        [PXUnboundDefault()]
        public virtual decimal? UsedExpense { get; set; }
        public abstract class usedExpense : PX.Data.BQL.BqlDecimal.Field<usedExpense> { }
        #endregion

        #region BudgetAmt 
        [PXDecimal(0)]
        [PXUIField(DisplayName = "Budget Amount", IsReadOnly = true)]
        [PXUnboundDefault()]
        public virtual decimal? BudgetAmt { get; set; }
        public abstract class budgetAmt : PX.Data.BQL.BqlDecimal.Field<budgetAmt> { }
        #endregion
    }
}
