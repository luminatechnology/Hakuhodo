using PX.Data;
using PX.Data.Update;
using PX.Objects.GL;
using UCG_Customization.Utils;
using UCG_Customization.Descriptor;
using PX.Objects.EP;
using PX.Objects.PM;
using static PX.Objects.AP.APRegisterUCGExt;

namespace PX.Objects.AP
{
    public class APInvoiceEntryUCGExt : PXGraphExtension<PX.Objects.AP.APInvoiceEntry>
    {
        #region Override
        public delegate void PersistDelegate();
        [PXOverride]
        public virtual void Persist(PersistDelegate baseMethod)
        {
            var now = Base.Document.Current;
            baseMethod();
            if (now == null) return;
            //當Approved 時，回到EP503010
            if (now.GetExtension<APRegisterUCGExt>()?.IsApproving == true && Base.Accessinfo.ScreenID == "AP.30.10.00")
            {
                throw new PXRedirectRequiredException(PXGraph.CreateInstance<EPApprovalProcess>(), "EPApprovalProcess");
            }
        }
        #endregion

        #region Event
        #region APInvoice
        protected virtual void _(Events.RowSelected<APInvoice> e, PXRowSelected baseMethod)
        {
            if (e.Row == null) return;
            baseMethod?.Invoke(e.Cache, e.Args);
            Base.payInvoice.SetCaption(PXLocalizer.Localize(Messages.APPayBill));
        }

        protected virtual void _(Events.FieldUpdated<APInvoice, APInvoice.branchID> e)
        {
            if (e.Row == null) return;

            if (e.Row.ProjectID != ProjectDefaultAttribute.NonProject())
                e.Cache.SetValueExt<APInvoice.projectID>(e.Row, null);
        }
        #endregion

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

        #region CacheAttached

        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXRemoveBaseAttribute(typeof(APDocStatus.ListAttribute))]
        [APDocStatusExt.List]
        protected virtual void _(Events.CacheAttached<APInvoice.status> e) { }
        #endregion

        #region Method
        protected virtual void ValidateUsedExpense(APTran tran)
        {
            if (tran == null) return;
            var rowExt = tran.GetExtension<APTranUCGExt>();
            if (rowExt.UsedExpense > rowExt.BudgetAmt)
                ErrorMsg.SetError<APTranUCGExt.usedExpense>(Base.Transactions.Cache, tran, rowExt.UsedExpense, "超出預算", PXErrorLevel.Warning);
        }
        #endregion
    }
}