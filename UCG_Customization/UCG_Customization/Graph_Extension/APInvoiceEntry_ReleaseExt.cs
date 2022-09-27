using PX.Data;
using PX.Objects.PM;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;
using PX.Objects.CA;

namespace PX.Objects.AP
{
    public class APInvoiceEntry_ReleaseExt : PXGraphExtension<PX.Objects.AP.APInvoiceEntry>
    {
        #region const
        #region User Defined
        /// <summary>User Defined - 折讓/退傭單號</summary>
        const string DJNBR = "AttributeDJNBR";
        /// <summary>User Defined - 代墊/暫借沖銷金額</summary>
        const string EPPAYAMT = "AttributeEPPAYAMT";
        /// <summary>User Defined - 暫借款單號</summary>
        const string EPPAYREFNB = "AttributeEPPAYREFNB";
        /// <summary>User Defined - 代墊款/暫借款</summary>
        const string EPPAYTYPE = "AttributeEPPAYTYPE";
        /// <summary>員工暫借款-代碼</summary>
        const string EMP_TEMP_BOR_CODE = "B";
        #endregion
        #region Message
        const string MSG_EPREFNBR_NOT_FOUND = "查無暫借款{0}";
        const string MSG_EPREFNBR_NOT_FOUND_4DATE = "申請日期早於暫借款{0}立帳日，請確認日期";
        const string MSG_EPPAYAMT_INSUFFICIENT = "暫借款{0}餘額不足";
        const string MSG_CANNOT_AUTO_ADJ = "暫借款 {0} 無法自動沖銷，請人工作業";
        const string MSG_PLZ_EPLENT = "請維護執行項目EPLENT";
        const string MSG_EPLENT_NOT_FOUND = "銀行帳戶查無對應的EPLENT";
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
            return baseMethod(adapter);
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
                    if (epPayType == EMP_TEMP_BOR_CODE)
                    {
                        //暫借款單號
                        string epPayRefNbr = (PXStringState)cache.GetValueExt(invoice, EPPAYREFNB);
                        //暫借沖銷金額
                        string _epPayAmt = (PXStringState)cache.GetValueExt(invoice, EPPAYAMT);
                        decimal epPayAmt = Decimal.Parse(_epPayAmt);
                        using (PXTransactionScope ts = new PXTransactionScope())
                        {
                            APInvoice ppmInvoice = ValidateEmpTempBor(invoice, epPayRefNbr, epPayAmt);
                            string epLentAP = CreateAPInvoice(invoice, ppmInvoice, epPayRefNbr, epPayAmt);
                            cache.SetValueExt<APRegisterUCGExt.usrEPLentAP>(invoice, epLentAP);
                            _return = baseMethod(adapter);
                            CreateAPPayment(invoice, epPayAmt);
                            ts.Complete();
                        }
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

        #region Method
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

        private string CreateAPInvoice(APInvoice inv, APInvoice ppm, string epPayRefNbr, decimal epPayAmt)
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

        private void CreateAPPayment(APInvoice inv, decimal epPayAmt)
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
            CashAccount cashAccount = GetEPLENT_CashAccount(graph, inv.BranchID);
            if (cashAccount == null) throw new PXException(MSG_EPLENT_NOT_FOUND);
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
            //set origamt to zero to apply "full" amounts to invoices.
            adj = PXCache<APAdjust>.CreateCopy(graph.Adjustments.Insert(adj));
            PXCache adCache = graph.Adjustments.Cache;
            adj = graph.Adjustments.Update(adj);
            adCache.SetValueExt<APAdjust.curyAdjgAmt>(adj, epPayAmt);
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

        private CashAccount GetEPLENT_CashAccount(PXGraph graph, int? branchID)
        {
            return PXSelect<CashAccount,
                Where<CashAccount.branchID, Equal<Required<CashAccount.branchID>>,
                And<CashAccount.cashAccountCD, Like<Required<CashAccount.cashAccountCD>>>>>
                .Select(graph, branchID, "EPLENT%");
        }
        #endregion
    }
}

