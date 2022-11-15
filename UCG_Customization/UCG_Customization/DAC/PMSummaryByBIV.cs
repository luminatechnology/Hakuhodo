using System;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;

namespace UCG_Customization.DAC
{
    [Serializable]
    [PXCacheName("PMSummaryByBIV")]
    public class PMSummaryByBIV : IBqlTable
    {
        #region Key
        public new class PK : PrimaryKeyOf<PMSummaryByBIV>.By<contractID>
        {
            public static PMSummaryByBIV Find(PXGraph graph, int? contractID) => FindBy(graph, contractID);
        }
        #endregion

        #region ContractID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Contract ID")]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : PX.Data.BQL.BqlInt.Field<contractID> { }
        #endregion

        #region UsedIncome
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Used Income",IsReadOnly = true)]
        public virtual Decimal? UsedIncome { get; set; }
        public abstract class usedIncome : PX.Data.BQL.BqlDecimal.Field<usedIncome> { }
        #endregion

        #region UsedExpense
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Used Expense", IsReadOnly = true)]
        public virtual Decimal? UsedExpense { get; set; }
        public abstract class usedExpense : PX.Data.BQL.BqlDecimal.Field<usedExpense> { }
        #endregion

        #region BudgetIncome
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Budget Income", IsReadOnly = true)]
        public virtual Decimal? BudgetIncome { get; set; }
        public abstract class budgetIncome : PX.Data.BQL.BqlDecimal.Field<budgetIncome> { }
        #endregion

        #region BudgetExpense
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Budget Expense", IsReadOnly = true)]
        public virtual Decimal? BudgetExpense { get; set; }
        public abstract class budgetExpense : PX.Data.BQL.BqlDecimal.Field<budgetExpense> { }
        #endregion

        #region Unbound
        #region UsedMargin 
        [PXDecimal(0)]
        [PXUIField(DisplayName = "Used Margin", IsReadOnly = true)]
        [PXFormula(typeof(Sub<usedIncome, usedExpense>))]
        public virtual decimal? UsedMargin { get; set; }
        public abstract class usedMargin : PX.Data.BQL.BqlDecimal.Field<usedMargin> { }
        #endregion

        #region UsedMarginPer 
        [PXDecimal(2)]
        [PXUIField(DisplayName = "Used Margin%", IsReadOnly = true)]
        public virtual decimal? UsedMarginPer
        {
            get
            {
                if (UsedIncome > 0m) return 100 * UsedMargin / UsedIncome;
                return 0m;
            }
        }
        public abstract class usedMarginPer : PX.Data.BQL.BqlDecimal.Field<usedMarginPer> { }
        #endregion

        #region BudgetMargin 
        [PXDecimal(0)]
        [PXUIField(DisplayName = "Budget Margin", IsReadOnly = true)]
        [PXFormula(typeof(Sub<budgetIncome, budgetExpense>))]
        public virtual decimal? BudgetMargin { get; set; }
        public abstract class budgetMargin : PX.Data.BQL.BqlDecimal.Field<budgetMargin> { }
        #endregion

        #region BudgetMarginPer 
        [PXDecimal(2)]
        [PXUIField(DisplayName = "Budget Margin%", IsReadOnly = true)]
        public virtual decimal? BudgetMarginPer
        {
            get
            {
                if (BudgetIncome > 0m) return 100 * BudgetMargin / BudgetIncome;
                return 0m;
            }
        }
        public abstract class budgetMarginPer : PX.Data.BQL.BqlDecimal.Field<budgetMarginPer> { }
        #endregion
        #endregion
    }
}