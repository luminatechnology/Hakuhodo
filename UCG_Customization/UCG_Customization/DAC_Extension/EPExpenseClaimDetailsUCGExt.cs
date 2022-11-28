using PX.Data;
using PX.Objects.IN;

namespace PX.Objects.EP
{
    public class EPExpenseClaimDetailsUCGExt : PXCacheExtension<EPExpenseClaimDetails>
    {

        #region UsrMiscExpItem
        //2022-11-17 Alton mark:[未來CR]
        //[Inventory(DisplayName = "MiscExp Item")]
        //[PXRestrictor(typeof(Where<InventoryItem.itemType, Equal<INItemTypes.expenseItem>>), Messages.InventoryItemIsNotAnExpenseType)]
        //[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        ////[PXUIRequired(typeof(Where<Selector<EPExpenseClaimDetails.inventoryID, InventoryItemUCGExt.usrIsMiscExp>, Equal<True>>))]
        //public virtual int? UsrMiscExpItem { get; set; }
        //public abstract class usrMiscExpItem : PX.Data.BQL.BqlInt.Field<usrMiscExpItem> { }
        #endregion

        #region Unbound
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
        #endregion
    }
}
