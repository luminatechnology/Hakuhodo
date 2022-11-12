using System;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CA;
using System.Collections.Generic;
using PX.Objects.CR;
using System.IO;
using System.Text;
using UCG_Customization.DAC;
using UCG_Customization.Utils;
using PX.Objects.AP;
using PX.Objects.AR;
using eGUICustomizations.DAC;
using ARRegisterAlias = PX.Objects.AR.Standalone.ARRegisterAlias;

namespace UCG_Customization
{
    public class LMTeleTransProcess : PXGraph<LMTeleTransProcess>
    {

        const string ENCODING = "BIG5";

        public PXCancel<MasterTable> Cancel;

        public PXFilter<MasterTable> MasterView;
        public PXFilteredProcessing<APARTeleTransView, MasterTable,
            Where<APARTeleTransView.branch, Equal<Current<MasterTable.branchID>>,
                    And<APARTeleTransView.cashAccountID, Equal<Current<MasterTable.cashAccountID>>,
                    And<APARTeleTransView.paymentMethodID, Equal<Current<MasterTable.payTypeID>>,
                    And<IsNull<APARTeleTransView.isTTGenerated, False>, Equal<Current<MasterTable.isTTGenerated>>,
                    And2<Where<Current2<MasterTable.docDate>, IsNull,
                       Or<Current2<MasterTable.docDate>, Equal<APARTeleTransView.docDate>>>,
                    And<Where<Current2<MasterTable.baccountID>, IsNull,
                        Or<Current2<MasterTable.baccountID>, Equal<APARTeleTransView.bAccountID>>>
            >>>>>>> DetailsView;

        #region Constructor
        public LMTeleTransProcess()
        {
            DetailsView.SetProcessAllCaption("Process All");
            //DetailsView.SetProcessAllEnabled(false);
            DetailsView.SetProcessCaption("Process");
        }
        #endregion

        #region Event
        protected void _(Events.RowSelected<MasterTable> e)
        {
            LMTeleTransProcess _this = this;
            DetailsView.SetProcessDelegate(delegate (List<APARTeleTransView> list) {
                DoProcess(list, _this);
            });
        }

        protected void _(Events.RowUpdated<MasterTable> e)
        {
            var row = e.Row;
            if (row == null) return;
            row.SelCount = 0;
            row.CurySelTotal = 0;
            DetailsView.Cache.Clear();
            //DetailsView.View.Clear();
        }

        protected void _(Events.FieldUpdated<APARTeleTransView, APARTeleTransView.selected> e)
        {
            var row = e.Row;
            if (row == null) return;
            DoSummary(row);
        }
        #endregion

        #region Process Method
        public static bool DoProcess(List<APARTeleTransView> datas, LMTeleTransProcess graph)
        {
            MasterTable param = graph.MasterView.Current;
            var sysDate = graph.Accessinfo.BusinessDate;

            if (datas == null || datas.Count == 0) return true;
            if (param.PayDate == null)
            {
                var msg = "Please input PayDate.";
                PXProcessing<APARTeleTransView>.SetError(msg);
                throw new PXException(msg);
            }
            Branch branch =(Branch) PXSelectorAttribute.Select<MasterTable.branchID>(graph.MasterView.Cache, param);
            string fileName = branch.BranchCD + "-" + param.PayDate?.ToString("yyyyMMdd") + ".data";

            param.SelCount = datas.Count;
            decimal total = 0;
            datas.ForEach(data=> total += (data.CuryOrigDocAmt??0));
            param.CurySelTotal = total;
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (StreamWriter sw = new StreamWriter(stream, Encoding.GetEncoding(ENCODING)))//BIG5
                    {
                        sw.WriteLine(DoHeader(datas, param, sysDate));
                        using (PXTransactionScope ts = new PXTransactionScope())
                        {
                            foreach (var data in datas)
                            {
                                DoPayLine(data, param, sw);
                                graph.UpdateUsrIsTTGenerated(data);
                            }
                            ts.Complete();
                        }
                        sw.Close();
                    }
                    throw new PXRedirectToFileException(
                       new PX.SM.FileInfo(
                           Guid.NewGuid(),
                           fileName,
                           null,
                           stream.ToArray(),
                           string.Empty),
                       true);
                }
            }
            catch (PXRedirectToFileException)
            {
                PXProcessing<APARTeleTransView>.SetProcessed();
                throw;
            }
            catch (Exception ex)
            {
                PXProcessing<APARTeleTransView>.SetError(ex);
                throw;
            }
        }
        #endregion

        #region Method
        private void DoSummary(APARTeleTransView row)
        {
            int basic = row.Selected ?? false ? 1 : -1;
            var param = MasterView.Current;
            param.SelCount += basic;
            param.CurySelTotal += ((row.CuryOrigDocAmt ?? 0) * basic);
        }

        private static string DoHeader(List<APARTeleTransView> datas, MasterTable param, DateTime? sysDate)
        {
            var segmentKey = "1"; //Segment Key (1)
            var ourCustomerID = GetStr(datas[0].TaxRegistrationID, 10, false);//OurCustomerID (10)
            var selCount = GetStr(param.SelCount.ToString(), 8, true, '0');//SelCount (8) Left pad with 0
            var curySelTotal = GetStr((param.CurySelTotal ?? 0).ToString("0"), 11, true,'0');//CurySelTotal (11) Left pad with 0
            var payBankBranch = GetStr(datas[0].PayBankBranch, 7, false);//PayBankBranch (7)
            var payAccount = GetStr(datas[0].ExtRefNbr, 16, false);//PayAccount (16)
            //2022-10-24 Alton BatchDate改抓param的paymentDate
            //var batchDate = GetStr(GetRocDateStr(sysDate), 7, false);//BatchDate (7) systemDat YYYmmDD (民國年)
            var batchDate = GetStr(GetRocDateStr(param.PayDate), 7, false);//BatchDate (7) systemDat YYYmmDD (民國年)
            var reserveField = GetStr("", 7, false);//ReserveField (7) left pad with blank
            var feePayee = GetStr(param.PayTypeID == "TTO" ? "1" : " ", 1, false);//FeePayee (1) if payment method = 'TTO' then 1 ,else blank
            var payName = GetStr("", 80, true);//PayName (80) left pad with blank
            return
                  segmentKey      //Segment Key (1)
                + ourCustomerID   //OurCustomerID (10)
                + selCount        //SelCount (8) Left pad with 0
                + curySelTotal    //CurySelTotal (11) Left pad with 0
                + payBankBranch   //PayBankBranch (7)
                + payAccount      //PayAccount (16)
                + batchDate       //BatchDate (7) systemDat YYYmmDD (民國年)
                + reserveField    //ReserveField (7) left pad with blank
                + feePayee        //FeePayee (1) if payment method = 'TTO' then 1 ,else blank
                + payName;        //PayName (80) left pad with blank
        }

        private static void DoPayLine(APARTeleTransView data, MasterTable param, StreamWriter sw)
        {
            bool isAP = data.DataType == "AP";
            if (isAP) { //AP聯絡資訊轉換
                APPayment payment = APPayment.PK.Find(new PXGraph(),data.DocType, data.RefNbr);
                APAddress addr = GetAPAddress(payment?.RemitAddressID);//取得isDefAddres = false資料
                APContact contact = GetAPContact(payment?.RemitContactID);//取得idDefContact = false資料
                if (addr != null) data.PostalCode = addr.PostalCode;
                if (contact != null) data.EMail = contact.Email;
            }

            var segmentKey = "2"; //(1)
            var vendorID = GetStr(data.BAccountTWID?.ToString(), 10, false);//(10) VendorID/CustomerID 
            var vendorName = GetStr(data.AcctName, 60, false);//(60) 
            var postCode = GetStr(data.PostalCode, 5, false);//(5)
            var email = GetStr(data.EMail, 80, false);//(80)
            var fax = GetStr("", 15, true);//(15)
            var bankBranch = GetStr(data.BankBranch, 7, false);//(7)
            var payAccount = GetStr(data.PayAccount, 16, false);//(16)
            var payAmount = GetStr((data.CuryOrigDocAmt ?? 0).ToString("0"), 11, true, '0');//(11) Left pad with 0
            var batchNbr = GetStr(data.BatchNbr, 10, false);//(10)
            var paymentDate = GetStr(GetRocDateStr(param.PayDate), 7, false);//(7)
            var paymentMod = " ";//(1)
            var remark = GetStr(data.DocDesc, 80, false);//(80)
            var notifyMethod = "14";//(2)
            var checkPeriod = "  ";//(2)
            var _lineDetailCount = 0;//Left pad with 0 

            //====Get detail
            List<String> detailStrs = new List<String>();
            if (isAP)
            {
                var details = GetAPTranPost(data.RefNbr, data.DocType);
                _lineDetailCount = details.Count;
                foreach (var tran in details)
                {
                    detailStrs.Add(DoLineDetail(tran));
                }
            }
            else
            {
                var details = GetARTranPost(data.RefNbr, data.DocType);
                _lineDetailCount = details.Count;
                foreach (var tran in details)
                {
                    detailStrs.Add(DoLineDetail(tran));
                }
            }

            var lineDetailCount = GetStr(_lineDetailCount.ToString(), 4, true, '0');//(4) Left pad with 0 
            var lineStr =
                  segmentKey    //(1)
                + vendorID      //(10)
                + vendorName    //(60)
                + postCode      //(5)
                + email         //(80)
                + fax           //(15)
                + bankBranch    //(7)
                + payAccount    //(16)
                + payAmount     //(11)
                + batchNbr      //(10)
                + paymentDate   //(7)
                + paymentMod    //(1)
                + remark        //(80)
                + notifyMethod  //(2)
                + checkPeriod   //(2)
                + lineDetailCount //(4)
                ;
            sw.WriteLine(lineStr);
            detailStrs.ForEach(sw.WriteLine);
        }

        private static string DoLineDetail(APTranPost tran)
        {
            var guiAPBill = GetTWNManualGUIAPBill(tran.SourceRefNbr, tran.SourceDocType);
            var srcAPRegister = GetSrcAPRegister(tran.SourceRefNbr, tran.SourceDocType);
            var segmentKey = "3";
            var serialNbr = GetStr((tran.ID ?? 0).ToString(), 4, true, '0');//(4)
            var item = GetStr(guiAPBill?.GUINbr, 10, false);//(10)
            var amtChar = (tran.CuryAmt ?? 0) >= 0 ? '0' : ' ';
            var amount = GetStr((tran.CuryAmt ?? 0).ToString("0"), 11, true, amtChar);//(11)
            var _remark = srcAPRegister?.DocDesc;
            if (_remark == null || "".Equals(_remark)) {
                _remark = GetAPTran(tran.SourceRefNbr, tran.SourceDocType)?.TranDesc;
            }
            var remark = GetStr(_remark, 80, false);//(80)
            var guiNbr = GetStr(guiAPBill?.GUINbr, 20, false);//(20)
            return
                  segmentKey
                + serialNbr
                + item
                + amount
                + remark
                + guiNbr;
        }
        private static string DoLineDetail(ARTranPost tran)
        {
            var guiTrans = GetTWNGUITrans(tran.SourceRefNbr, tran.SourceDocType);
            var srcARRegister = GetSrcARRegister(tran.SourceRefNbr, tran.SourceDocType);
            var segmentKey = "3";
            var serialNbr = GetStr((tran.ID ?? 0).ToString(), 4, true, '0');//(4)
            var item = GetStr(guiTrans?.GUINbr, 10, false);//(10)
            var amtChar = (tran?.CuryAmt ?? 0) >= 0 ? '0' : ' ';//TODO: 待確認欄位資訊
            var amount = GetStr((tran?.CuryAmt ?? 0).ToString("0"), 11, true, amtChar);//(11)
            var _remark = srcARRegister?.DocDesc;
            if (_remark == null || "".Equals(_remark))
            {
                _remark = GetARTran(tran.SourceRefNbr, tran.SourceDocType)?.TranDesc;
            }
            var remark = GetStr(_remark, 80, false);//(80)
            var guiNbr = GetStr(guiTrans?.GUINbr, 20, false);//(20)
            return
                  segmentKey
                + serialNbr
                + item
                + amount
                + remark
                + guiNbr;
        }

        private static string GetStr(string str, int lenght, bool isLeft, char paddingChar = ' ')
        {
            return TextUtil.GetByByte(str, lenght, isLeft, paddingChar, ENCODING);
        }

        private static string GetRocDateStr(DateTime? date)
        {
            if (date == null) return "";
            int? yyy = date?.Year - 1911;
            return
                  yyy.ToString().PadLeft(3, '0')
                + date?.Month.ToString().PadLeft(2, '0')
                + date?.Day.ToString().PadLeft(2, '0');
        }

        protected void UpdateUsrIsTTGenerated(APARTeleTransView data)
        {
            if (data.DataType == "AP")
            {
                PXUpdate<
                    Set<APPaymentTTExt.usrIsTTGenerated, True>,
                    APPayment,
                    Where<APPayment.refNbr, Equal<Required<APPayment.refNbr>>,
                    And<APPayment.docType, Equal<Required<APPayment.docType>>>>>
                    .Update(this, data.RefNbr, data.DocType);
            }
            else
            {
                PXUpdate<
                    Set<ARPaymentTTExt.usrIsTTGenerated, True>,
                    ARPayment,
                    Where<ARPayment.refNbr, Equal<Required<ARPayment.refNbr>>,
                    And<ARPayment.docType, Equal<Required<ARPayment.docType>>>>>
                    .Update(this, data.RefNbr, data.DocType);
            }
        }

        #endregion

        #region BQL
        private static PXResultset<APTranPost> GetAPTranPost(string refNbr, string docType)
        {
            return PXSelect<APTranPost,
                   Where<APTranPost.refNbr,NotEqual<APTranPost.sourceRefNbr>,
                       And<APTranPost.docType,NotEqual<APTranPost.sourceDocType>, 
                       And<APTranPost.refNbr, Equal<Required<APTranPost.refNbr>>,
                       And<APTranPost.docType, Equal<Required<APTranPost.docType>>>>>>>
                       .Select(new PXGraph(), refNbr, docType);
        }

        private static PXResultset<ARTranPost> GetARTranPost(string refNbr, string docType)
        {
            return PXSelect<ARTranPost,
                   Where< ARTranPost.refNbr,NotEqual<ARTranPost.sourceRefNbr>,
                       And<ARTranPost.docType, NotEqual<ARTranPost.sourceDocType>,
                       And<ARTranPost.refNbr, Equal<Required<ARTranPost.refNbr>>,
                       And<ARTranPost.docType, Equal<Required<ARTranPost.docType>>>>>>>
                   .Select(new PXGraph(), refNbr, docType);
        }

        private static TWNManualGUIAPBill GetTWNManualGUIAPBill(string srcRefNbr, string srcDocType)
        {
            return PXSelect<TWNManualGUIAPBill,
                   Where<TWNManualGUIAPBill.refNbr, Equal<Required<TWNManualGUIAPBill.refNbr>>,
                        And<TWNManualGUIAPBill.docType, Equal<Required<TWNManualGUIAPBill.docType>>>>,
                   OrderBy<Desc<TWNManualGUIAPBill.netAmt>>>
                   .Select(new PXGraph(), srcRefNbr, srcDocType);
        }

        private static TWNGUITrans GetTWNGUITrans(string srcRefNbr, string srcDocType)
        {
            return PXSelect<TWNGUITrans,
                   Where<TWNGUITrans.orderNbr, Equal<Required<TWNGUITrans.orderNbr>>,
                        And<TWNGUITrans.docType, Equal<Required<TWNGUITrans.docType>>>>>
                   .Select(new PXGraph(), srcRefNbr, srcDocType);
        }

        private static APRegister GetSrcAPRegister(string srcRefNbr, string srcDocType)
        {
            return PXSelect<APRegister,
                   Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>,
                       And<APRegister.docType, Equal<Required<APRegister.docType>>>>>
                   .Select(new PXGraph(), srcRefNbr, srcDocType);
        }

        private static ARRegister GetSrcARRegister(string srcRefNbr, string srcDocType)
        {
            return PXSelect<ARRegister,
                   Where<ARRegister.refNbr, Equal<Required<ARRegister.refNbr>>,
                       And<ARRegister.docType, Equal<Required<ARRegister.docType>>>>>
                   .Select(new PXGraph(), srcRefNbr, srcDocType);
        }

        private static APAddress GetAPAddress(int? addressID)
        {
            return PXSelect<APAddress,
                   Where<APAddress.isDefaultAddress, Equal<False>,
                       And<APAddress.addressID, Equal<Required<APAddress.addressID>>>>>
                   .Select(new PXGraph(), addressID);
        }

        private static APContact GetAPContact(int? contactID)
        {
            return PXSelect<APContact,
                   Where<APContact.isDefaultContact, Equal<False>,
                       And<APContact.contactID, Equal<Required<APContact.contactID>>>>>
                   .Select(new PXGraph(), contactID);
        }

        private static APTran GetAPTran(string srcRefNbr,string srcDocType) {
            return PXSelect<APTran,
                   Where<APTran.refNbr, Equal<Required<APTran.refNbr>>,
                   And<APTran.tranType, Equal<Required<APTran.tranType>>>>,
                   OrderBy<Asc<APTran.lineNbr>>>
                   .Select(new PXGraph(),srcRefNbr,srcDocType);
        }

        private static ARTran GetARTran(string srcRefNbr, string srcDocType)
        {
            return PXSelect<ARTran,
                   Where<ARTran.refNbr, Equal<Required<ARTran.refNbr>>,
                   And<ARTran.tranType, Equal<Required<ARTran.tranType>>>>,
                   OrderBy<Asc<ARTran.lineNbr>>>
                   .Select(new PXGraph(), srcRefNbr, srcDocType);
        }

        #endregion

        #region Table
        [Serializable]
        [PXCacheName("TeleTrans Filter")]
        public class MasterTable : IBqlTable
        {
            #region Param
            #region BranchID
            [PXInt()]
            [PXUIField(DisplayName = "BranchID", Required = true)]
            [PXUnboundDefault(typeof(AccessInfo.branchID))]
            [PXDimensionSelector("BIZACCT", typeof(Search<Branch.branchID, Where<Branch.active, Equal<True>, And<MatchWithBranch<Branch.branchID>>>>), typeof(Branch.branchCD), DescriptionField = typeof(Branch.acctName))]
            public virtual int? BranchID { get; set; }
            public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
            #endregion

            #region PayTypeID
            [PXString()]
            [PXUIField(DisplayName = "PayTypeID", Required = true)]
            [PXSelector(typeof(Search<PaymentMethod.paymentMethodID,
                          Where<PaymentMethod.isActive, Equal<True>>>),
                SubstituteKey = typeof(PaymentMethod.paymentMethodID),
                DescriptionField = typeof(PaymentMethod.descr)
                )]
            public virtual string PayTypeID { get; set; }
            public abstract class payTypeID : PX.Data.BQL.BqlString.Field<payTypeID> { }
            #endregion

            #region CashAccountID
            [PXInt()]
            [PXUIField(DisplayName = "PayAccountID", Required = true)]
            [PXSelector(typeof(Search2<CashAccount.cashAccountID, 
                InnerJoin<PaymentMethodAccount,On<CashAccount.cashAccountID,Equal<PaymentMethodAccount.cashAccountID>>>,
                Where<CashAccount.active, Equal<True>,
                And<CashAccount.branchID,Equal<Current<branchID>>,
                    And<PaymentMethodAccount.paymentMethodID,Equal<Current<payTypeID>>>>>>),
                SubstituteKey = typeof(CashAccount.cashAccountCD),
                DescriptionField = typeof(CashAccount.descr))]
            public virtual int? CashAccountID { get; set; }
            public abstract class cashAccountID : PX.Data.BQL.BqlInt.Field<cashAccountID> { }
            #endregion

            #region DocDate
            [PXDate()]
            [PXUIField(DisplayName = "DocDate")]
            [PXUnboundDefault(typeof(AccessInfo.businessDate))]
            public virtual DateTime? DocDate { get; set; }
            public abstract class docDate : PX.Data.BQL.BqlDateTime.Field<docDate> { }
            #endregion

            #region BAccountID
            [PXInt()]
            [PXUIField(DisplayName = "BAccountID")]
            [PXSelector(typeof(BAccountR.bAccountID),
                SubstituteKey = typeof(BAccountR.acctCD),
                DescriptionField = typeof(BAccountR.acctName)
                )]
            public virtual int? BAccountID { get; set; }
            public abstract class baccountID : PX.Data.BQL.BqlInt.Field<baccountID> { }
            #endregion

            #region IsTTGenerated
            [PXBool()]
            [PXUIField(DisplayName = "Is TT Generated")]
            [PXUnboundDefault(false)]
            public virtual bool? IsTTGenerated { get; set; }
            public abstract class isTTGenerated : PX.Data.BQL.BqlBool.Field<isTTGenerated> { }
            #endregion
            #endregion

            #region Summary
            #region PayDate
            [PXDate()]
            [PXUIField(DisplayName = "PayDate", Required = true)]
            [PXDefault(typeof(AccessInfo.businessDate))]
            public virtual DateTime? PayDate { get; set; }
            public abstract class payDate : PX.Data.BQL.BqlDateTime.Field<payDate> { }
            #endregion

            #region CurySelTotal
            [PXDecimal(0)]
            [PXUIField(DisplayName = "CurySelTotal", IsReadOnly = true)]
            public virtual decimal? CurySelTotal { get; set; }
            public abstract class curySelTotal : PX.Data.BQL.BqlDecimal.Field<curySelTotal> { }
            #endregion

            #region SelCount
            [PXInt()]
            [PXUIField(DisplayName = "SelCount", IsReadOnly = true)]
            public virtual int? SelCount { get; set; }
            public abstract class selCount : PX.Data.BQL.BqlInt.Field<selCount> { }
            #endregion
            #endregion
        }
        #endregion


    }
}