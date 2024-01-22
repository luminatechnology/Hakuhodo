using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;
using eGUICustomizations.Graph;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace eGUICustomizationsByBranch.Graph
{
    public class TWNGenerateNHIFileBase : PXGraph<TWNGenerateNHIFileBase>
    {
        public const string Comma = ",";

        #region Features
        public PXCancel<TWNGenerateNHIFileFilter> Cancel;
        public PXFilter<TWNGenerateNHIFileFilter> Filter;
        public SelectFrom<TWNWHTTran>.Where<TWNWHTTran.branchID.IsEqual<TWNGenerateNHIFileFilter.branchID.FromCurrent>
                                            .And<TWNWHTTran.paymDate.IsBetween<TWNGenerateNHIFileFilter.fromDate.FromCurrent,
                                                                               TWNGenerateNHIFileFilter.toDate.FromCurrent>
                                                 .And<TWNGenerateNHIFileFilter.secNHICode.FromCurrent.Contains<PX.Data.RTrim<TWNWHTTran.secNHICode>>
                                                      .And<TWNWHTTran.secNHIAmt.IsGreater<decimal0>>>>>
                                     .ProcessingView.FilteredBy<TWNGenerateNHIFileFilter> WHTTran;
        #endregion

        #region Ctor
        public TWNGenerateNHIFileBase()
        {
            WHTTran.SetProcessCaption(ActionsMessages.Export);
            WHTTran.SetProcessAllCaption(TWMessages.ExportAll);
            WHTTran.SetProcessDelegate(Export);
        }
        #endregion

        #region Methods
        protected virtual void Export(List<TWNWHTTran> trans) 
        {
            string lines = string.Empty;

            var filter = Filter.Current;
            var graph  = CreateInstance<TWNGenGUIMediaFile>();

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(stream, Encoding.Unicode))
                {
                    BAccountExt bAcctExt = BAccountExt.GetTWGUIByBranch(WHTTran.Cache, filter.BranchID);

                    string fileName = $"DPR{bAcctExt.UsrOurTaxNbr}{graph.GetTWNDate(this.Accessinfo.BusinessDate.Value, true)}A{filter.SecNHIType}.csv";
                    string conditionKey = null;

                    int totalCount = 0, rowCount = 1;
                    decimal totalNetAmt = 0m, totalNHIAmt = 0m;

                    // Lines
                    foreach (TWNWHTTran tran in trans.OrderBy(o => o.PersonalID).ThenBy(t => t.PaymDate).ToList()) 
                    {
                        // 資料識別碼
                        lines += $"2{Comma}";
                        // 處理方式(新增I  覆蓋R)
                        lines += $"{filter.ProcessingMethod}{Comma}";
                        // 給付日期
                        lines += $"{graph.GetTWNDate(tran.PaymDate.Value, true)}{Comma}";
                        // 所得人身分證號 
                        lines += $"{tran.PersonalID}{Comma}";
                        // 所得人姓名
                        lines += $"{tran.PayeeName}{Comma}";
                        // 單次給付金額
                        lines += $"{Math.Round(tran.NetAmt.Value, 0, MidpointRounding.AwayFromZero)}{Comma}";
                        // 扣繳補充保險費金額
                        lines += $"{Math.Round(tran.SecNHIAmt.Value,0, MidpointRounding.AwayFromZero)}{Comma}";
                        // 每一筆預設編列『1』，惟當同一所得人同一給付日同一所得類別有2筆以上者(不論所得金額有無相同)，則第2筆的申報編號編列『2』，第3筆的申報編號編列『3』，依此類推
                        if (conditionKey != $"{tran.PersonalID}-{tran.PaymDate.Value.Date}") { rowCount = 1; }
                        lines += $"{rowCount}{Comma}";
                        // Blank
                        lines += new string(Convert.ToChar(Comma), 3) + "\r\n";

                        rowCount++;
                        totalCount++;
                        totalNetAmt += tran.NetAmt.Value;
                        totalNHIAmt += tran.SecNHIAmt.Value;

                        conditionKey = $"{tran.PersonalID}-{tran.PaymDate.Value.Date}";
                    }

                    // Header
                    // 資料識別碼
                    string headerLine = $"1{Comma}";
                    // 統一編號
                    headerLine += $"{bAcctExt.UsrOurTaxNbr}{Comma}";
                    // 所得類別
                    headerLine += $"{filter.SecNHIType}{Comma}";
                    // 給付起始年月
                    headerLine += $"{graph.GetTWNDate(filter.FromDate.Value, false)}{Comma}";
                    // 給付結束年月
                    headerLine += $"{graph.GetTWNDate(filter.ToDate.Value, false)}{Comma}";
                    // 申報總筆數
                    headerLine += $"{totalCount}{Comma}";
                    // 所得(收入)給付總額
                    headerLine += $"{Math.Round(totalNetAmt, 0, MidpointRounding.AwayFromZero)}{Comma}";
                    // 扣繳補充保險費總額
                    headerLine += $"{Math.Round(totalNHIAmt, 0, MidpointRounding.AwayFromZero)}{Comma}";
                    // 扣費義務人
                    headerLine += $"{bAcctExt.UsrPersonInCharge}{Comma}";
                    // 聯絡電話
                    headerLine += $"{bAcctExt.UsrCompanyPhone}{Comma}";
                    // 電子郵件信箱
                    var contact = (ContactBAccount)SelectFrom<ContactBAccount>.InnerJoin<Branch>.On<Branch.bAccountID.IsEqual<ContactBAccount.bAccountID>>
                                                                              .Where<Branch.branchID.IsEqual<P.AsInt>>
                                                                              .View.SelectSingleBound(this, null, filter.BranchID);
                    headerLine += $"{contact.EMail}{Comma}";
                    // 聯絡人姓名
                    headerLine += $"{contact.Attention}{Comma}\r\n";

                    lines = headerLine + lines;

                    sw.Write(lines);
                    sw.Close();

                    DownloadCSVFile(fileName, stream);
                }
            }
        }

        protected virtual void DownloadCSVFile(string fileName, MemoryStream stream)
        {
            throw new PXRedirectToFileException(new PX.SM.FileInfo(Guid.NewGuid(), fileName,
                                                                    null, stream.ToArray(), string.Empty),
                                                true);
        }
        #endregion  
    }

    [Serializable]
    [PXCacheName("TWN Generate NHI File Filter")]
    public class TWNGenerateNHIFileFilter : IBqlTable
    {
        #region BranchID
        [Branch()]
        [PXDefault(typeof(AccessInfo.branchID))]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region FromDate
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "From Date", Visibility = PXUIVisibility.Visible)]
        public virtual DateTime? FromDate { get; set; }
        public abstract class fromDate : PX.Data.BQL.BqlDateTime.Field<fromDate> { }
        #endregion

        #region ToDate
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "To Date", Visibility = PXUIVisibility.Visible)]
        public virtual DateTime? ToDate { get; set; }
        public abstract class toDate : PX.Data.BQL.BqlDateTime.Field<toDate> { }
        #endregion

        #region SecNHICode
        [PXDBString(10, IsFixed = true)]
        [PXUIField(DisplayName = "2GNHI Code", Enabled = false)]
        [SecNHICodeSelector(ValidateValue = false)]
        public virtual string SecNHICode { get; set; }
        public abstract class secNHICode : PX.Data.BQL.BqlString.Field<secNHICode> { }
        #endregion

        #region ProcessingMethod
        [PXDBString(1, IsFixed = true)]
        [PXUIField(DisplayName = "Processing Method", Required = true)]
        [PXDefault("I")]
        [PXStringList(new string[] { "I", "R" }, new string[] { nameof(RecipientsBehaviorAttribute.Add), nameof(RecipientsBehaviorAttribute.Override) })]
        public virtual string ProcessingMethod { get; set; }
        public abstract class processingMethod : PX.Data.BQL.BqlString.Field<processingMethod> { }
        #endregion

        #region SecNHIType
        [PXDBString(2, IsFixed = true)]
        [PXUIField(DisplayName = "2GNHI Type", Enabled = false, Visible = false)]
        public virtual string SecNHIType { get; set; }
        public abstract class secNHIType : PX.Data.BQL.BqlString.Field<secNHIType> { }
        #endregion
    }
}
