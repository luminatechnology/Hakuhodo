﻿using PX.Data;
using PX.Data.Update;
using PX.Objects.AR;
using PX.Objects.CT;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using System;
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
            //當Approved 時，回到EP503010
            if (now.GetExtension<EPExpenseClaimWorkGroupExt>()?.IsApproving == true && Base.Accessinfo.ScreenID == "EP.30.10.00")
            {
                throw new PXRedirectRequiredException(PXGraph.CreateInstance<EPApprovalProcess>(), "EPApprovalProcess");
            }
        }
        #endregion

        #region Event
        #region EPExpenseClaim
        protected virtual void _(Events.RowSelected<EPExpenseClaim> e)
        {
            if (e.Row == null) return;
            //明細專案不可改，改由表頭專案控制
            PXUIFieldAttribute.SetReadOnly<EPExpenseClaimDetails.contractID>(Base.ExpenseClaimDetails.Cache, null, true);

            //2022-11-17 Alton mark:[未來CR]
            //PXCache detailCache = Base.ExpenseClaimDetails.Cache;
            //foreach (EPExpenseClaimDetails detail in Base.ExpenseClaimDetails.Select())
            //{
            //    InventoryItem item = (InventoryItem)PXSelectorAttribute.Select<EPExpenseClaimDetails.inventoryID>(detailCache, detail);
            //    //Item是否為雜支
            //    bool isMiscExpItem = item?.GetExtension<InventoryItemUCGExt>()?.UsrIsMiscExp == true;
            //    SetRequired<EPExpenseClaimDetailsUCGExt.usrMiscExpItem>(detailCache, detail, isMiscExpItem);
            //    PXUIFieldAttribute.SetReadOnly<EPExpenseClaimDetailsUCGExt.usrMiscExpItem>(detailCache, detail, !isMiscExpItem);
            //}

        }

        protected virtual void _(Events.FieldUpdated<EPExpenseClaim, EPExpenseClaimWorkGroupExt.usrProjectID> e)
        {
            if (e.Row == null) return;
            foreach (EPExpenseClaimDetails item in Base.ExpenseClaimDetails.Select())
            {
                Base.ExpenseClaimDetails.Cache.SetDefaultExt<EPExpenseClaimDetails.contractID>(item);
                Base.ExpenseClaimDetails.Cache.SetDefaultExt<EPExpenseClaimDetails.taskID>(item);
                if (Base.ExpenseClaimDetails.Cache.GetStatus(item) == PXEntryStatus.Notchanged)
                    Base.ExpenseClaimDetails.Cache.SetStatus(item, PXEntryStatus.Updated);
            }
        }
        #endregion

        #region EPExpenseClaimDetails
        protected virtual void _(Events.RowPersisted<EPExpenseClaimDetails> e)
        {
            if (e.Row == null) return;
            e.Cache.SetDefaultExt<EPExpenseClaimDetailsUCGExt.usedExpense>(e.Row);
            e.Cache.SetDefaultExt<EPExpenseClaimDetailsUCGExt.budgetAmt>(e.Row);
        }

        protected virtual void _(Events.RowInserting<EPExpenseClaimDetails> e, PXRowInserting baseHandler)
        {
            if (e.Row == null) return;
            baseHandler?.Invoke(e.Cache, e.Args);
            e.Cache.SetDefaultExt<EPExpenseClaimDetails.contractID>(e.Row);
            e.Cache.SetDefaultExt<EPExpenseClaimDetails.taskID>(e.Row);
            e.Cache.SetDefaultExt<EPExpenseClaimDetails.customerID>(e.Row);
            e.Cache.SetDefaultExt<EPExpenseClaimDetails.customerLocationID>(e.Row);
            e.Cache.SetDefaultExt<EPExpenseClaimDetails.billable>(e.Row);
        }

        protected virtual void _(Events.RowSelected<EPExpenseClaimDetails> e)
        {
            if (e.Row == null) return;
            ValidateUsedExpense(e.Row);
        }

        protected virtual void _(Events.FieldDefaulting<EPExpenseClaimDetails, EPExpenseClaimDetails.taskID> e ,PXFieldDefaulting baseMethod)
        {
            if (e.Row == null) return;
            //UAT & PRD nonProject 莫名有對應的 Default TaskID，因此nonProject直接預設清空
            if (e.Row.ContractID != null && ProjectDefaultAttribute.IsNonProject(e.Row.ContractID))
            {
                e.NewValue = null;
            }
            else {
                baseMethod?.Invoke(e.Cache,e.Args);
            }
        }

        #region 2022-11-17 Alton mark:[未來CR]
        protected virtual void _(Events.FieldUpdated<EPExpenseClaimDetails, EPExpenseClaimDetails.inventoryID> e, PXFieldUpdated baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);
            if (e.Row == null) return;
            //2022-11-17 Alton mark:[未來CR]
            //InventoryItem item = (InventoryItem)PXSelectorAttribute.Select<EPExpenseClaimDetails.inventoryID>(e.Cache, e.Row);
            //if (item?.GetExtension<InventoryItemUCGExt>()?.UsrIsMiscExp != true)
            //{
            //    e.Cache.SetValueExt<EPExpenseClaimDetailsUCGExt.usrMiscExpItem>(e.Row, null);
            //}
        }

        //2022-11-17 Alton mark:[未來CR]
        //protected virtual void _(Events.FieldUpdated<EPExpenseClaimDetails, EPExpenseClaimDetailsUCGExt.usrMiscExpItem> e, PXFieldUpdated baseMethod)
        //{
        //    if (e.Row == null) return;
        //    var rowExt = e.Row.GetExtension<EPExpenseClaimDetailsUCGExt>();
        //    if (rowExt.UsrMiscExpItem == null)
        //    {
        //        e.Cache.SetDefaultExt<EPExpenseClaimDetails.taxCategoryID>(e.Row);
        //    }
        //    else
        //    {
        //        InventoryItem item = (InventoryItem)PXSelectorAttribute.Select<EPExpenseClaimDetailsUCGExt.usrMiscExpItem>(e.Cache, e.Row);
        //        e.Cache.SetValueExt<EPExpenseClaimDetails.taxCategoryID>(e.Row, item.TaxCategoryID);
        //    }
        //}
        #endregion

        protected virtual void _(Events.FieldDefaulting<EPExpenseClaimDetails, EPExpenseClaimDetails.contractID> e, PXFieldDefaulting baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            if (e.Row == null) return;
            e.NewValue = Base.ExpenseClaim.Current.GetExtension<EPExpenseClaimWorkGroupExt>().UsrProjectID;
        }

        protected virtual void _(Events.FieldDefaulting<EPExpenseClaimDetails, EPExpenseClaimDetails.customerLocationID> e, PXFieldDefaulting baseHandler)
        {
            if (e.Row == null) return;
            if (!ProjectDefaultAttribute.IsNonProject(e.Row.ContractID))
            {
                Contract projectCust = PXSelectorAttribute.Select<EPExpenseClaimDetails.contractID>(e.Cache, e.Row) as Contract;
                var custoemr = Customer.PK.Find(Base, projectCust?.CustomerID);
                e.NewValue = custoemr?.DefLocationID;
            }
            else
            {
                baseHandler?.Invoke(e.Cache, e.Args);
            }
        }

        protected virtual void _(Events.FieldDefaulting<EPExpenseClaimDetails, EPExpenseClaimDetails.billable> e, PXFieldDefaulting baseHandler)
        {
            if (e.Row == null) return;
            baseHandler?.Invoke(e.Cache, e.Args);
            if (e.Row.CustomerLocationID != null)
            {
                e.NewValue = true;
            }
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
                   account?.AccountGroupID);
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
                   account?.AccountGroupID);
        }

        #endregion

        #region CacheAttached
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(typeof(EPExpenseClaimWorkGroupExt.usrProjectID))]
        protected virtual void _(Events.CacheAttached<EPExpenseClaimDetails.contractID> e) { }
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

        protected virtual void SetRequired<Field>(PXCache cache, object item, bool required) where Field : IBqlField
        {
            PXPersistingCheck type = required ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing;
            PXUIFieldAttribute.SetRequired<Field>(cache, required);
            PXDefaultAttribute.SetPersistingCheck<Field>(cache, item, type);
        }

        /// <summary>
        /// 判斷是否簽核中
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsApproving()
        {
            foreach (EPApproval approval in Base.Approval.Select())
            {
                if (approval.Status == EPApprovalStatus.Approved || approval.Status == EPApprovalStatus.Rejected)
                {
                    EPApproval old = GetOldEPApproval(approval.ApprovalID);
                    if (old?.Status == EPApprovalStatus.Pending)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region BQL
        protected virtual PXResultset<EPApproval> GetOldEPApproval(int? approvalID)
        {
            // Acuminator disable once PX1072 PXGraphCreationForBqlQueries [Justification]
            // Acuminator disable once PX1003 NonSpecificPXGraphCreateInstance [Justification]
            return PXSelect<EPApproval, Where<EPApproval.approvalID, Equal<Required<EPApproval.approvalID>>>>
                .Select(new PXGraph(), approvalID);
        }
        #endregion
    }
}
