using PX.Data;
using PX.Objects.PM;
using PX.Objects.IN;
using PX.Objects.GL;
using PX.Objects.CR;
using System;
using System.Collections;
using System.Collections.Generic;
using PX.Objects.CA;
using PX.Common;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using UCG_Customization.Utils;

namespace PX.Objects.AP
{
    /**
     * 廠商代墊/員工代墊/員工暫借
     * 
     * 2022-12-28 - 員工代墊邏輯改為與廠商代墊相同
     * **/
    public class APInvoiceEntry_ReleaseExt : PXGraphExtension<PX.Objects.AP.APInvoiceEntry>
    {
        #region const
        #region User Defined
        /// <summary>User Defined - 折讓/退傭單號</summary>
        public const string DJNBR = "AttributeDJNBR";
        /// <summary>User Defined - 代墊/暫借沖銷金額</summary>
        public const string EPPAYAMT = "AttributeEPPAYAMT";
        /// <summary>User Defined - 暫借款單號</summary>
        public const string EPPAYREFNB = "AttributeEPPAYREFNB";
        /// <summary>User Defined - 代墊款/暫借款</summary>
        public const string EPPAYTYPE = "AttributeEPPAYTYPE";
        /// <summary>User Defined - 代墊廠商</summary>
        public const string VENDOR = "AttributeVENDOR";
        /// <summary>員工代墊款-代碼</summary>
        public const string EMP_ADV_CODE = "A";
        /// <summary>員工暫借款-代碼</summary>
        public const string EMP_TEMP_BOR_CODE = "B";
        /// <summary>廠商代墊-代碼</summary>
        public const string VEN_ADV_CODE = "C";
        #endregion
        #region Message
        const string MSG_EPREFNBR_NOT_FOUND = "查無暫借款{0}";
        const string MSG_EPREFNBR_NOT_FOUND_4DATE = "申請日期早於暫借款{0}立帳日，請確認日期";
        const string MSG_EPPAYAMT_INSUFFICIENT = "代墊/暫借款{0}餘額不足";
        const string MSG_EPPAYAMT_FORMAT_ERR = "代墊/暫借款：僅供輸入數字";
        const string MSG_EPPAYAMT_GREATER_AMT = "代墊/暫借款大於請款金額，請確認金額";
        const string MSG_EPPAYAMT_ZERO_AMT = "代墊金額不得為0";
        const string MSG_CANNOT_AUTO_ADJ = "暫借款 {0} 無法自動沖銷，請人工作業";
        const string MSG_PLZ_EPLENT = "請維護執行項目EPLENT";
        const string MSG_CASH_ACCOUNT_NOT_FOUND = "銀行帳戶查無對應的{0}";
        const string MSG_VENDOR_CAN_NOT_SAME = "代墊廠商/員工 與 供應商 不能相同";
        #endregion
        #endregion

        #region Event Handlers
        #region ReleaseFromHold
        public delegate IEnumerable ReleaseFromHoldDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable ReleaseFromHold(PXAdapter adapter, ReleaseFromHoldDelegate baseMethod)
        {
            APInvoice invoice = Base.Document.Current;
            PXCache cache = Base.Document.Cache;
            string epPayType = (PXStringState)cache.GetValueExt(invoice, EPPAYTYPE);
            //代墊款/暫借款 是否為 員工暫借款
            if (epPayType == EMP_TEMP_BOR_CODE)
            {
                //暫借款單號
                string epPayRefNbr = (PXStringState)cache.GetValueExt(invoice, EPPAYREFNB);
                //暫借沖銷金額
                string _epPayAmt = (PXStringState)cache.GetValueExt(invoice, EPPAYAMT);
                decimal epPayAmt = Decimal.Parse(_epPayAmt);
                ValidateEmpTempBor(invoice, epPayRefNbr, epPayAmt);
            }
            var _return = baseMethod(adapter);
            Base.Persist();
            return _return;
        }
        #endregion

        #region Release
        public delegate IEnumerable ReleaseDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable Release(PXAdapter adapter, ReleaseDelegate baseMethod)
        {
            IEnumerable _return = adapter.Get();
            PXLongOperation.StartOperation(Base, delegate ()
            {
                try
                {
                    APInvoice invoice = Base.Document.Current;
                    PXCache cache = Base.Document.Cache;
                    string epPayType = (PXStringState)cache.GetValueExt(invoice, EPPAYTYPE);
                    //代墊款/暫借款 是否為 員工暫借款
                    if (epPayType.IsIn(EMP_ADV_CODE, EMP_TEMP_BOR_CODE, VEN_ADV_CODE))
                    {
                        DoCustRelease(adapter, baseMethod, invoice, cache, epPayType);
                    }
                    else
                    {
                        _return = baseMethod(adapter);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return _return;
        }
        #endregion

        #endregion

        #region Event
        protected virtual void _(Events.RowPersisting<APInvoice> e)
        {
            var row = e.Row;
            if (row == null) return;
            APSetupUCGExt setupExt = Base.APSetup.Current.GetExtension<APSetupUCGExt>();
            if (setupExt.UsrIsCheckAdvVendor == true) ValidateAdvAcct(e.Cache, row);
            if (setupExt.UsrIsCheckAdvAmt == true) ValidateAdvAmt(e.Cache, row);

        }
        #endregion

        #region Method
        /// <summary>
        /// 檢核代墊廠商/員工 與 AP供應商 不能相同
        /// </summary>
        protected virtual void ValidateAdvAcct(PXCache cache, APInvoice row)
        {
            string epPayType = (PXStringState)cache.GetValueExt(row, EPPAYTYPE);
            if (epPayType.IsIn(EMP_ADV_CODE, VEN_ADV_CODE))
            {
                //代墊廠商/代墊員工
                string vendor = (PXStringState)cache.GetValueExt(row, VENDOR);
                BAccount baccount = GetBaccountByCD(Base, vendor);
                if (row.VendorID == baccount?.BAccountID)
                {
                    ErrorMsg.SetError<APInvoice.vendorID>(cache, row, vendor, MSG_VENDOR_CAN_NOT_SAME);
                    ErrorMsg.SetError(cache, VENDOR, row, vendor, MSG_VENDOR_CAN_NOT_SAME);
                }
            }
        }

        /// <summary>
        /// 檢核 當選擇員工代墊款或廠商代墊款時，代墊金額不得為0
        /// </summary>
        protected virtual void ValidateAdvAmt(PXCache cache, APInvoice row)
        {
            string epPayType = (PXStringState)cache.GetValueExt(row, EPPAYTYPE);
            if (epPayType.IsIn(EMP_ADV_CODE, VEN_ADV_CODE))
            {
                string _epPayAmt = (PXStringState)cache.GetValueExt(row, EPPAYAMT);
                decimal epPayAmt = 0;
                string error = null;
                try
                {
                    epPayAmt = Decimal.Parse(_epPayAmt);
                }
                catch (Exception)
                {
                    error = MSG_EPPAYAMT_FORMAT_ERR;
                }
                if (epPayAmt == 0m)
                {
                    error = MSG_EPPAYAMT_ZERO_AMT;
                }
                else if (epPayAmt > row.CuryOrigDocAmt)
                {
                    error = MSG_EPPAYAMT_GREATER_AMT;
                }
                if (error != null) ErrorMsg.SetError(cache, EPPAYAMT, row, _epPayAmt, error);
            }
        }

        private void DoCustRelease(PXAdapter adapter, ReleaseDelegate baseMethod, APInvoice invoice, PXCache cache, string epPayType)
        {

            //暫借款單號
            string epPayRefNbr = (PXStringState)cache.GetValueExt(invoice, EPPAYREFNB);
            //暫借沖銷金額
            string _epPayAmt = (PXStringState)cache.GetValueExt(invoice, EPPAYAMT);
            decimal epPayAmt = 0;
            try
            {
                epPayAmt = Decimal.Parse(_epPayAmt);
            }
            catch (Exception)
            {
                throw new PXException(MSG_EPPAYAMT_FORMAT_ERR);
            }
            if (epPayAmt > invoice.CuryOrigDocAmt)
            {
                throw new PXException(MSG_EPPAYAMT_GREATER_AMT);
            }

            using (PXTransactionScope ts = new PXTransactionScope())
            {
                if (epPayType == EMP_TEMP_BOR_CODE)
                {
                    APInvoice ppmInvoice = ValidateEmpTempBor(invoice, epPayRefNbr, epPayAmt);
                    string epLentAP = CreateAPInvoice4TempBor(invoice, ppmInvoice, epPayRefNbr, epPayAmt);
                    cache.SetValueExt<APRegisterUCGExt.usrEPLentAP>(invoice, epLentAP);
                    baseMethod(adapter);
                    CreateAPPayment4TempBor(invoice, epPayAmt);
                }
                else if (epPayType.IsIn(EMP_ADV_CODE, VEN_ADV_CODE))
                {
                    //代墊廠商/代墊員工
                    string vendor = (PXStringState)cache.GetValueExt(invoice, VENDOR);
                    BAccount baccount = GetBaccountByCD(Base, vendor);
                    int? vendorID = baccount.BAccountID;

                    //產生APInvoice
                    string epLentAP = CreateAPInvoice4Adv(invoice, epPayAmt, vendorID, epPayType);
                    cache.SetValueExt<APRegisterUCGExt.usrEPLentAP>(invoice, epLentAP);
                    baseMethod(adapter);
                    //產生APPayment
                    CreateAPPayment4Adv(invoice, epPayAmt, epPayType);
                }

                ts.Complete(Base);
            }
        }

        /// <summary>
        /// 驗證員工暫借款
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="epPayType"></param>
        /// <param name="epPayRefNbr"></param>
        /// <param name="epPayAmt"></param>
        private APInvoice ValidateEmpTempBor(APInvoice invoice, string epPayRefNbr, decimal epPayAmt)
        {

            APInvoice ppmInvoice = APInvoice.PK.Find(Base, APDocType.Prepayment, epPayRefNbr);
            //A檢核 暫借款須為PPM 且狀態為 OPEN 且ppmDocDate <= AP.DocDate
            if (ppmInvoice == null || ppmInvoice.Status != APDocStatus.Open || ppmInvoice.BranchID != invoice.BranchID)
            {
                throw new PXException(MSG_EPREFNBR_NOT_FOUND, epPayRefNbr);
            }
            else if (ppmInvoice.DocDate > invoice.DocDate)
            {
                throw new PXException(MSG_EPREFNBR_NOT_FOUND_4DATE, epPayRefNbr);
            }
            if (ppmInvoice.CuryDocBal < epPayAmt)
            {
                throw new PXException(MSG_EPPAYAMT_INSUFFICIENT, epPayRefNbr);
            }
            return ppmInvoice;
        }

        private string CreateAPInvoice4TempBor(APInvoice inv, APInvoice ppm, string epPayRefNbr, decimal epPayAmt)
        {
            APInvoiceEntry graph = PXGraph.CreateInstance<APInvoiceEntry>();
            //c.建立 一張新的AP單
            APInvoice invoice = graph.Document.Current = graph.Document.Insert();
            invoice = graph.Document.Update(invoice);
            PXCache cache = graph.Document.Cache;
            cache.SetValueExt<APInvoice.docType>(invoice, APDocType.Invoice);
            //    日期 = 原AP(#77) 文件日期
            cache.SetValueExt<APInvoice.docDate>(invoice, inv.DocDate);
            //    供應商 = 暫借款單(#63)-供應商
            cache.SetValueExt<APInvoice.vendorID>(invoice, ppm.VendorID);
            //    Branch = 原AP Branch
            cache.SetValueExt<APInvoice.branchID>(invoice, inv.BranchID);
            //    專案 = X
            cache.SetValueExt<APInvoice.projectID>(invoice, ProjectDefaultAttribute.NonProject());
            //    說明 = 原AP(#77)“– 暫借款沖銷”
            cache.SetValueExt<APInvoice.docDesc>(invoice, inv.RefNbr + " – 暫借款沖銷");
            //    UserDefinedField - 折讓 / 退傭單號 = ‘NA’
            cache.SetValueExt(invoice, DJNBR, "NA");
            //    經辦人 = ADMIN(emp = 24)
            cache.SetValueExt<APInvoice.employeeID>(invoice, 24);
            invoice = graph.Document.Update(invoice);

            //    明細tab插入一筆 執行項目 = EPLENT  數量1、金額 = 上面寫的沖銷金額
            #region --Transactions
            InventoryItem inventoryItem = GetInvotryByCD(graph, "EPLENT");
            if (inventoryItem == null) throw new PXException(MSG_PLZ_EPLENT);
            APTran tran = graph.Transactions.Insert();
            tran = graph.Transactions.Update(tran);
            PXCache tCache = graph.Transactions.Cache;
            tCache.SetValueExt<APTran.inventoryID>(tran, inventoryItem.InventoryID);
            tran.Qty = 1;
            tCache.SetValueExt<APTran.qty>(tran, tran.Qty);
            tCache.SetValueExt<APTran.curyUnitCost>(tran, epPayAmt);
            tran = graph.Transactions.Update(tran);

            #endregion
            //    沖銷tab 應該會自動有一筆預付款(單號 = 暫借款單號) 支付金額 = 上面寫的沖銷金額
            #region --Adjustments
            APInvoiceEntry.APAdjust adj = null;
            PXCache adCache = graph.Adjustments.Cache;
            foreach (APInvoiceEntry.APAdjust _adj in graph.Adjustments.Select())
            {
                //檢查沖銷tab是否有暫借款單號
                if (_adj.DisplayRefNbr == epPayRefNbr)
                {
                    adj = _adj;
                    break;
                }
            }
            //沖銷tab 沒有該筆預付款單號: 報錯: 暫借款 APxxxx 無法自動沖銷，請人工作業
            if (adj == null) throw new PXException(MSG_CANNOT_AUTO_ADJ, epPayRefNbr);
            adCache.SetValueExt<APInvoiceEntry.APAdjust.curyAdjdAmt>(adj, epPayAmt);
            graph.Adjustments.Update(adj);
            #endregion
            graph.Save.Press();
            graph.releaseFromHold.Press();
            graph.release.Press();
            return graph.Document.Current.RefNbr;
        }

        private void CreateAPPayment4TempBor(APInvoice inv, decimal epPayAmt)
        {
            APPaymentEntry graph = PXGraph.CreateInstance<APPaymentEntry>();
            //建立一張 APPayment(CHK)
            APPayment payment = graph.Document.Current = graph.Document.Insert();
            payment = graph.Document.Update(payment);
            PXCache cache = graph.Document.Cache;
            cache.SetValueExt<APPayment.docType>(payment, APDocType.Check);
            //Payment Date = #77文件日期
            cache.SetValueExt<APPayment.adjDate>(payment, inv.DocDate);
            cache.SetValueExt<APPayment.docDate>(payment, inv.DocDate);

            cache.SetValueExt<APPayment.vendorID>(payment, inv.VendorID);
            //Branch = 原 AP Branch
            cache.SetValueExt<APPayment.branchID>(payment, inv.BranchID);

            //Payment Method = 原AP Payment Method
            cache.SetValueExt<APPayment.paymentMethodID>(payment, inv.PayTypeID);
            //Pay Account = by 不同branch有各自對應的 EPLENTXX
            CashAccount cashAccount = GetCashAccountByLikeCD(graph, inv.BranchID, "EPLENT%");
            if (cashAccount == null) throw new PXException(MSG_CASH_ACCOUNT_NOT_FOUND, "EPLENT");
            cache.SetValueExt<APPayment.cashAccountID>(payment, cashAccount.CashAccountID);
            //Pay Amount = 沖銷金額
            cache.SetValueExt<APPayment.curyOrigDocAmt>(payment, epPayAmt);
            //Description = 原AP單號(#77) – 暫借款沖銷
            cache.SetValueExt<APPayment.docDesc>(payment, inv.RefNbr + " – 暫借款沖銷");
            payment = graph.Document.Update(payment);

            #region Adjustments
            //下方待沖帳文件 應該要有原AP(#77) 更新支付金額 = 沖銷金額

            APAdjust adj = new APAdjust();
            adj.AdjdDocType = inv.DocType;
            adj.AdjdRefNbr = inv.RefNbr;
            adj.AdjdLineNbr = 0;
            adj = PXCache<APAdjust>.CreateCopy(graph.Adjustments.Insert(adj));
            PXCache adCache = graph.Adjustments.Cache;
            adj = graph.Adjustments.Update(adj);
            adCache.SetValueExt<APAdjust.curyAdjgAmt>(adj, epPayAmt);
            adCache.SetValueExt<APAdjust.curyAdjgWhTaxAmt>(adj, 0m);
            //待沖帳文件 查無(#77)時，報錯: 請檢查XXXX
            #endregion
            //提交、放行單據  正確放行 END
            graph.Save.Press();
            //提交
            graph.releaseFromHold.Press();
            //放行
            graph.release.Press();

        }

        private string CreateAPInvoice4Adv(APInvoice inv, decimal epPayAmt, int? vendorID, string epPayType)
        {
            APInvoiceEntry graph = PXGraph.CreateInstance<APInvoiceEntry>();
            //建立 一張新的AP單
            APInvoice invoice = graph.Document.Current = graph.Document.Insert();
            invoice = graph.Document.Update(invoice);
            PXCache cache = graph.Document.Cache;
            cache.SetValueExt<APInvoice.docType>(invoice, APDocType.Invoice);
            ////Vendor = 原單 經辦人
            //BAccount baccount = GetBaccountByDefContact(graph, inv.EmployeeID);
            cache.SetValueExt<APInvoice.vendorID>(invoice, vendorID);
            //DocDate = 原單DocDate
            cache.SetValueExt<APInvoice.docDate>(invoice, inv.DocDate);
            //供應商參考 = 原單RefNbr
            cache.SetValueExt<APInvoice.invoiceNbr>(invoice, inv.RefNbr);
            //Doc Desc = ‘員工代墊 -’ +原單DocDesc
            string desc = epPayType == EMP_ADV_CODE ? "員工代墊" : "廠商代墊";
            cache.SetValueExt<APInvoice.docDesc>(invoice, desc + " - " + inv.DocDesc);
            //折讓退傭單號 = ‘NA’
            cache.SetValueExt(invoice, DJNBR, "NA");
            //財務Branch = 原單Branch
            cache.SetValueExt<APInvoice.branchID>(invoice, inv.BranchID);

            //專案 = X
            cache.SetValueExt<APInvoice.projectID>(invoice, ProjectDefaultAttribute.NonProject());
            cache.SetValueExt<APInvoice.dueDate>(invoice, inv.DueDate);
            //經辦人 = ADMIN(emp = 24)
            cache.SetValueExt<APInvoice.employeeID>(invoice, 24);
            var taxZone = TX.TaxZone.PK.Find(Base, "TAXABLE");
            //2023-02-20 員工代墊 稅額預設為TAXABLE，為了排除代扣稅
            if (taxZone != null) cache.SetValueExt<APInvoice.taxZoneID>(invoice, taxZone.TaxZoneID);

            invoice = graph.Document.Update(invoice);

            //Item
            #region --Transactions
            APTran tran = new APTran()
            {
                RefNbr = invoice.RefNbr,
                TranType = invoice.DocType
            };
            tran = graph.Transactions.Insert(tran);
            PXCache tCache = graph.Transactions.Cache;
            //Branch = 原單Branch
            tCache.SetValueExt<APTran.branchID>(tran, inv.BranchID);
            //TranDesc = ‘員工代墊 -’ +原單DocDesc
            tCache.SetValueExt<APTran.tranDesc>(tran, desc + " - " + inv.DocDesc);
            //成本小計 = 代墊金額
            tCache.SetValueExt<APTran.qty>(tran, 1m);
            tCache.SetValueExt<APTran.curyUnitCost>(tran, epPayAmt);

            //會計科目 = 2229001
            Account account = Account.UK.Find(graph, "2229001");
            if (account == null) throw new PXException("請維護會計科目：2229001");
            tCache.SetValueExt<APTran.accountID>(tran, account.AccountID);

            //子科目 = 000000000
            Sub sub = Sub.UK.Find(graph, "000000000");
            if (sub == null) throw new PXException("請維護子科目：000000000");
            tCache.SetValueExt<APTran.subID>(tran, sub.SubID);
            //稅務類別 = EXAMPT
            tCache.SetValueExt<APTran.taxCategoryID>(tran, "EXAMPT");
            tran = graph.Transactions.Update(tran);

            #region 原單APTran
            foreach (APTran item in GetAPTrans(graph, inv.RefNbr, inv.DocType))
            {
                APTran orgTran = new APTran()
                {
                    RefNbr = invoice.RefNbr,
                    TranType = invoice.DocType
                };
                orgTran = graph.Transactions.Insert(orgTran);
                //Branch = 原單Branch
                tCache.SetValueExt<APTran.branchID>(orgTran, inv.BranchID);
                //TranDesc = ‘員工代墊 -’ +原單DocDesc
                tCache.SetValueExt<APTran.tranDesc>(orgTran, item.TranDesc);
                //成本小計 = 代墊金額
                tCache.SetValueExt<APTran.qty>(orgTran, item.Qty);
                tCache.SetValueExt<APTran.curyUnitCost>(orgTran, 0m);
                //會計科目 = 2229001
                tCache.SetValueExt<APTran.accountID>(orgTran, account.AccountID);
                //子科目 = 000000000
                tCache.SetValueExt<APTran.subID>(orgTran, sub.SubID);
                //稅務類別 = EXAMPT
                tCache.SetValueExt<APTran.taxCategoryID>(orgTran, "EXAMPT");
                orgTran = graph.Transactions.Update(orgTran);
            }

            #endregion
            #endregion

            graph.Save.Press();
            graph.releaseFromHold.Press();
            graph.release.Press();
            return graph.Document.Current.RefNbr;
        }

        private void CreateAPPayment4Adv(APInvoice inv, decimal epPayAmt, string epPayType)
        {
            APPaymentEntry graph = PXGraph.CreateInstance<APPaymentEntry>();
            //建立一張 APPayment(CHK)
            APPayment payment = graph.Document.Current = graph.Document.Insert();
            payment = graph.Document.Update(payment);
            PXCache cache = graph.Document.Cache;
            cache.SetValueExt<APPayment.docType>(payment, APDocType.Check);
            //供應商 = 原單Vendor
            cache.SetValueExt<APPayment.vendorID>(payment, inv.VendorID);
            //財務Branch = 原單branch
            cache.SetValueExt<APPayment.branchID>(payment, inv.BranchID);
            //沖帳日期 = docdate = 原單doc date
            cache.SetValueExt<APPayment.adjDate>(payment, inv.DocDate);
            cache.SetValueExt<APPayment.docDate>(payment, inv.DocDate);
            //付款方法 = 原單付款方法
            cache.SetValueExt<APPayment.paymentMethodID>(payment, inv.PayTypeID);
            //銀行帳戶 = EPPAYxx(依branch會有不同的代碼)
            CashAccount cashAccount = GetCashAccountByLikeCD(graph, inv.BranchID, "EPPAY%");
            if (cashAccount == null) throw new PXException(MSG_CASH_ACCOUNT_NOT_FOUND, "EPPAY");
            cache.SetValueExt<APPayment.cashAccountID>(payment, cashAccount.CashAccountID);
            //上方付款金額 = 代墊金額
            cache.SetValueExt<APPayment.curyOrigDocAmt>(payment, epPayAmt);
            string desc = epPayType == EMP_ADV_CODE ? "員工代墊" : "廠商代墊";
            cache.SetValueExt<APPayment.docDesc>(payment, desc + " - " + inv.DocDesc);
            payment = graph.Document.Update(payment);

            #region Adjustments
            //下方待沖帳文件 應該要有原AP(#77) 更新支付金額 = 沖銷金額
            APAdjust adj = new APAdjust();
            adj.AdjdDocType = inv.DocType;
            adj.AdjdRefNbr = inv.RefNbr;
            adj.AdjdLineNbr = 0;
            adj = PXCache<APAdjust>.CreateCopy(graph.Adjustments.Insert(adj));
            PXCache adCache = graph.Adjustments.Cache;
            adj = graph.Adjustments.Update(adj);
            adCache.SetValueExt<APAdjust.curyAdjgAmt>(adj, epPayAmt);
            adCache.SetValueExt<APAdjust.curyAdjgWhTaxAmt>(adj, 0m);
            //待沖帳文件 查無(#77)時，報錯: 請檢查XXXX
            #endregion
            //提交、放行單據  正確放行 END
            graph.Save.Press();
            //提交
            graph.releaseFromHold.Press();
            //放行
            graph.release.Press();
        }
        #endregion

        #region BQL
        private InventoryItem GetInvotryByCD(PXGraph graph, string inventoryCD)
        {
            return PXSelect<InventoryItem,
                Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>>
                .Select(graph, inventoryCD);
        }

        private CashAccount GetCashAccountByLikeCD(PXGraph graph, int? branchID, string cashAccountCD)
        {
            return PXSelect<CashAccount,
                Where<CashAccount.branchID, Equal<Required<CashAccount.branchID>>,
                And<CashAccount.cashAccountCD, Like<Required<CashAccount.cashAccountCD>>>>>
                .Select(graph, branchID, cashAccountCD);
        }

        private BAccount GetBaccountByDefContact(PXGraph graph, int? contactID)
        {
            return SelectFrom<BAccount>.Where<BAccount.defContactID.IsEqual<@P.AsInt>>.View.Select(graph, contactID);
        }

        private BAccount GetBaccountByCD(PXGraph graph, string acctCD)
        {
            return SelectFrom<BAccount>.Where<BAccount.acctCD.IsEqual<@P.AsString>>.View.Select(graph, acctCD);
        }

        private PXResultset<APTran> GetAPTrans(PXGraph graph, string refNbr, string docType)
        {
            return PXSelect<APTran, Where<APTran.refNbr, Equal<Required<APTran.refNbr>>,
                And<APTran.tranType, Equal<Required<APTran.tranType>>>>>
                .Select(graph, refNbr, docType);
        }


        #endregion
    }
}

