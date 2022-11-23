using PX.Data;
using System.Collections.Generic;
using UCG_Customization.Descriptor;

namespace PX.Objects.AP
{
    public class APTranUCGExt : PXCacheExtension<APTran>
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

        #region ReturnAmt 
        [PXDecimal(0)]
        [PXUIField(DisplayName = "Return Amount", IsReadOnly = true)]
        [APReturnAmount(APReturnAmountAttribute.Type.DETAIL)]

        public virtual decimal? ReturnAmt { get; set; }
        public abstract class returnAmt : PX.Data.BQL.BqlDecimal.Field<returnAmt> { }
        #endregion
    }
}
