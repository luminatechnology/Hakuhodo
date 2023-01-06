using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using PX.Common;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;

namespace eGUICustomizations.Graph
{
    public class TWNGenGUIMediaFile : PXGraph<TWNGenGUIMediaFile>
    {
        #region Variable Defintions
        public char   zero      = '0';
        public char   space     = ' ';       
        public int    fixedLen  = 0;
        public string combinStr = "";
        #endregion

        #region Features & Setup
        public PXCancel<GUITransFilter> Cancel; 
        public PXFilter<GUITransFilter> Filter;
        public PXFilteredProcessing<TWNGUITrans, 
                                    GUITransFilter,
                                    Where<TWNGUITrans.gUIDecPeriod, Between<Current<GUITransFilter.fromDate>, Current<GUITransFilter.toDate>>,
                                          And<TWNGUITrans.branchID, Equal<Current<GUITransFilter.branchID>>,
                                              And<Where2<Where<TWNGUITrans.gUIDirection, Equal<TWNStringList.TWNGUIDirection.receipt>,
                                                                And<TWNGUITrans.taxAmount, Greater<decimal0>>>,
                                                         Or<TWNGUITrans.gUIDirection, Equal<TWNStringList.TWNGUIDirection.issue>>>>>>,
                                    OrderBy<Asc<TWNGUITrans.gUIDecPeriod>>> GUITransList;
        public PXSetup<TWNGUIPreferences> gUIPreferSetup;
        #endregion

        #region Ctor
        public TWNGenGUIMediaFile()
        {
            GUITransList.SetProcessCaption(ActionsMessages.Export);
            GUITransList.SetProcessAllCaption(TWMessages.ExportAll);
            GUITransList.SetProcessDelegate(Export);
        }
        #endregion

        #region Constant String Classes
        public class GUI2x : PX.Data.BQL.BqlString.Constant<GUI2x>
        {
            public GUI2x() : base("GUI2%") { }
        }

        public class GUI3x : PX.Data.BQL.BqlString.Constant<GUI3x>
        {
            public GUI3x() : base("GUI3%") { }
        }
        #endregion

        #region Event Handler
        protected void _(Events.FieldDefaulting<GUITransFilter.toDate> e)
        {
            e.NewValue = Accessinfo.BusinessDate.Value.AddDays(1).Date.AddSeconds(-1);
        }
        #endregion

        #region Methods
        public virtual void Export(List<TWNGUITrans> tWNGUITrans)
        {
            try
            {
                TWNGUIPreferences gUIPreferences = gUIPreferSetup.Current;

                int    count = 1;
                string lines = "";

                using (MemoryStream stream = new MemoryStream())
                {
                    using (StreamWriter sw = new StreamWriter(stream, Encoding.ASCII))
                    {
                        string ourTaxNbr = BAccountExt.GetOurTaxNbByBranch(this.GUITransList.Cache, tWNGUITrans[0].BranchID);
                        string taxRegiID = BAccountExt.GetTWGUIByBranch(this.GUITransList.Cache, tWNGUITrans[0].BranchID)?.UsrTaxRegistrationID;

                        string fileName = $"{ourTaxNbr}.txt";

                        foreach (TWNGUITrans gUITrans in tWNGUITrans)
                        {
                            // Reporting Code
                            lines = gUITrans.GUIFormatCode;
                            // Tax Registration
                            lines += taxRegiID;
                            // Sequence Number
                            lines += AutoNumberAttribute.GetNextNumber(GUITransList.Cache, gUITrans, gUIPreferences.MediaFileNumbering, Accessinfo.BusinessDate);
                            // GUI LegalYM
                            lines += GetTWNDate(gUITrans.GUIDate.Value);
                            // Tax ID (Buyer)
                            lines += GetBuyerTaxID(gUITrans);
                            // Tax ID (Seller)
                            lines += GetSellerTaxID(gUITrans);
                            // GUI Number
                            lines += GetGUINbr(gUITrans);
                            // Net Amount
                            lines += GetNetAmt(gUITrans);
                            // Tax Group
                            lines += GetTaxGroup(gUITrans);
                            // Tax Amount
                            lines += GetTaxAmt(gUITrans);
                            // Deduction Code
                            lines += (gUITrans.DeductionCode != null || gUITrans.GUIFormatCode.StartsWith("2")) ? gUITrans.DeductionCode : new string(space, 1);
                            // Blank
                            lines += new string(space, 5);
                            // Special Tax Rate
                            lines += new string(space, 1);
                            // Summary Remark
                            lines += GetSummaryRemark(gUITrans);
                            // Export Method
                            lines += GetExportMethod(gUITrans);

                            // Only the last line does not need to be broken.
                            if (count < tWNGUITrans.Count)
                            {
                                sw.WriteLine(lines);
                                count++;
                            }
                            else
                            {
                                sw.Write(lines);
                            }
                        }

                        count = 1;
                        PXSelectBase<NumberingSequence> query = new PXSelect<NumberingSequence, Where<NumberingSequence.numberingID, Like<GUI2x>,
                                                                                                      Or<NumberingSequence.numberingID, Like<GUI3x>,
                                                                                                         And<NumberingSequence.startDate, GreaterEqual<Current<GUITransFilter.fromDate>>,
                                                                                                             And<NumberingSequence.startDate, LessEqual<Current<GUITransFilter.toDate>>>>>>>(this);

                        foreach (NumberingSequence numSeq in query.Select())
                        {
                            string StrEndNbr  = numSeq.EndNbr.Substring(2);
                            string StrLastNbr = numSeq.LastNbr.Substring(2);

                            int LastNbr = Convert.ToInt32(StrLastNbr);

                            if (numSeq.StartNbr == numSeq.LastNbr || LastNbr <= Convert.ToInt32(StrEndNbr))
                            {
                                lines = "\r\n";
                                // Reporting Code
                                lines += numSeq.NumberingID.Substring(numSeq.NumberingID.IndexOf('I') + 1, 2);
                                // Tax Registration
                                lines += taxRegiID;
                                // Sequence Number
                                lines += AutoNumberAttribute.GetNextNumber(query.Cache, numSeq, gUIPreferences.MediaFileNumbering, Accessinfo.BusinessDate);
                                // GUI LegalYM
                                lines += GetTWNDate(Filter.Current.ToDate.Value);
                                // Tax ID (Buyer)
                                lines += numSeq.EndNbr.Substring(2);
                                // Tax ID (Seller)
                                lines += ourTaxNbr;
                                // GUI Number
                                string Next2LastNbr = (LastNbr + 1).ToString();
                                lines += string.Format("{0}{1}", numSeq.LastNbr.Substring(0, 2), Next2LastNbr.Length < StrLastNbr.Length ? Next2LastNbr.PadLeft(StrLastNbr.Length, zero) : Next2LastNbr);
                                // Net Amount
                                lines += new string(zero, 12);
                                // Tax Group
                                lines += "D";
                                // Tax Amount
                                lines += new string(zero, 10);
                                // Deduction Code
                                lines += new string(space, 1);
                                // Blank
                                lines += new string(space, 5);
                                // Special Tax Rate
                                lines += new string(space, 1);
                                // Summary Remark
                                lines += "A";
                                // Export Method
                                lines += new string(space, 1);

                                sw.Write(lines);
                            }
                        }

                        sw.Close();

                        // Redirect browser to file created in memory on server
                        throw new PXRedirectToFileException(new PX.SM.FileInfo(Guid.NewGuid(), fileName,
                                                                                null, stream.ToArray(), string.Empty),
                                                            true);
                    }
                }
            }
            catch (PXException ex)
            {
                PXProcessing<TWNGUITrans>.SetError(ex);
                throw;
            }
        }

        public virtual string GetTWNDate(DateTime dateTime, bool fullDate = false)
        {
            var tWCalendar = new System.Globalization.TaiwanCalendar();

            int    year  = tWCalendar.GetYear(dateTime);
            string month = DateTime.Parse(dateTime.ToString()).ToString("MM");
            
            if (fullDate == true)
            {
                string day = DateTime.Parse(dateTime.ToString()).ToString("dd");

                return string.Format("{0}{1}{2}", year, month, day);
            }

            return string.Format("{0}{1}", year, month);
        }

        public virtual string GetNetAmt(TWNGUITrans gUITrans)
        {
            fixedLen = 12;

            bool sellerFmtCode = false, buyerFmtCode = false;

            string sellerTaxID = GetSellerTaxID(gUITrans);
            string buyerTaxID  = GetBuyerTaxID(gUITrans);

            if (gUITrans.GUIFormatCode.IsIn(TWGUIFormatCode.vATOutCode32, TWGUIFormatCode.vATInCode22, TWGUIFormatCode.vATInCode27) ) { sellerFmtCode = true; }

            if (gUITrans.GUIFormatCode.IsIn(TWGUIFormatCode.vATOutCode31, TWGUIFormatCode.vATOutCode35) ) { buyerFmtCode = true; }

            if (gUITrans.GUIStatus == TWNStringList.TWNGUIStatus.Voided)
            {
                combinStr = new string(zero, fixedLen);
            }
            else if (sellerFmtCode ||
                     (gUITrans.GUIFormatCode.Equals(TWGUIFormatCode.vATInCode25) && sellerTaxID.Length != 8) ||
                     (buyerFmtCode && (string.IsNullOrEmpty(buyerTaxID) || buyerTaxID == new string(space, 8)))
                    )
            {
                combinStr = (gUITrans.NetAmount + gUITrans.TaxAmount).ToString();
            }
            else
            {
                combinStr = gUITrans.NetAmount.ToString();
            }

            return combinStr.PadLeft(fixedLen, zero);
        }

        public virtual string GetGUINbr(TWNGUITrans gUITrans)
        {
            if (gUITrans.GUIFormatCode.IsIn(TWGUIFormatCode.vATInCode28, TWGUIFormatCode.vATInCode29))
            {
                return gUITrans.GUINbr.Substring(4, gUITrans.GUINbr.Length - 4);
            }
            else if (string.IsNullOrEmpty(gUITrans.GUINbr))
            {
                return new string(space, 10);
            }
            else
            {
                return gUITrans.GUINbr.Substring(0, 10);
            }
        }

        private string GetBuyerTaxID(TWNGUITrans gUITrans)
        {
            if (gUITrans.GUIFormatCode.StartsWith("2"))
            {
                return gUITrans.OurTaxNbr;
            }
            else if (gUITrans.GUIFormatCode.StartsWith("3") && gUITrans.GUIStatus != TWNStringList.TWNGUIStatus.Voided)
            {
                return gUITrans.TaxNbr ?? new string(space, 8);
            }

            return new string(space, 8);
        }

        private string GetSellerTaxID(TWNGUITrans gUITrans)
        {
            if (gUITrans.GUIFormatCode.StartsWith("2"))
            {
                if (gUITrans.GUIFormatCode.IsIn(TWGUIFormatCode.vATInCode28, TWGUIFormatCode.vATInCode29) )
                {
                    return (new string(space, 4) + gUITrans.GUINbr.Substring(0, 4));
                }
                else if (gUITrans.GUIFormatCode.IsIn(TWGUIFormatCode.vATInCode26, TWGUIFormatCode.vATInCode27) )
                {
                    return gUITrans.TaxNbr + new string(space, (8 - gUITrans.TaxNbr.Length));
                }
                else
                {
                    return gUITrans.TaxNbr ?? new string(space, 8);
                }
            }

            return gUITrans.OurTaxNbr;
        }

        private string GetTaxGroup(TWNGUITrans gUITrans)
        {
            if      (gUITrans.GUIStatus == TWNStringList.TWNGUIStatus.Voided) { return "F"; }
            else if (gUITrans.VATType == TWNStringList.TWNGUIVATType.Five)    { return "1"; }
            else if (gUITrans.VATType == TWNStringList.TWNGUIVATType.Zero)    { return "2"; }
            else if (gUITrans.VATType == TWNStringList.TWNGUIVATType.Exclude) { return "3"; }
            else                                                              { return new string(space, 1); }
        }

        private string GetTaxAmt(TWNGUITrans gUITrans)
        {
            fixedLen = 10;

            bool fixedFmtCode = false, sellerFmtCode = false, buyerFmtCode = false;

            string buyerTaxID = GetBuyerTaxID(gUITrans);

            if (gUITrans.GUIFormatCode.IsIn(TWGUIFormatCode.vATOutCode32, TWGUIFormatCode.vATInCode22, TWGUIFormatCode.vATInCode27) ) { fixedFmtCode = true; }

            if (gUITrans.GUIFormatCode == TWGUIFormatCode.vATInCode25 && GetSellerTaxID(gUITrans).Length != 8)  { sellerFmtCode = true; }

            if ((gUITrans.GUIFormatCode == TWGUIFormatCode.vATOutCode31 && (string.IsNullOrEmpty(buyerTaxID) || buyerTaxID == new string(space, 8)) ) ||
                (gUITrans.GUIFormatCode == TWGUIFormatCode.vATOutCode35 && (string.IsNullOrEmpty(buyerTaxID) || buyerTaxID == new string(space, 8)) )
               ) { buyerFmtCode = true; }

            if (gUITrans.GUIStatus == TWNStringList.TWNGUIStatus.Voided || fixedFmtCode || sellerFmtCode || buyerFmtCode)
            {
                return new string(zero, fixedLen);
            }
            else
            {
                combinStr = gUITrans.TaxAmount.ToString();

                return combinStr.PadLeft(fixedLen, zero);
            }
        }

        private string GetSummaryRemark(TWNGUITrans gUITrans)
        {
            if (gUITrans.GUIFormatCode.IsIn(TWGUIFormatCode.vATInCode26, TWGUIFormatCode.vATInCode27) ||
                gUITrans.GUIFormatCode == TWGUIFormatCode.vATInCode25 && GetSellerTaxID(gUITrans).Length != 8)
            {
                return "A";
            }
            
            return new string(space, 1);
        }

        private string GetExportMethod(TWNGUITrans gUITrans)
        {
            if (gUITrans.VATType == TWNStringList.TWNGUIVATType.Zero &&
                gUITrans.GUIStatus != TWNStringList.TWNGUIStatus.Voided &&
                gUITrans.SequenceNo == 0)
            {
                return gUITrans.CustomType == TWNStringList.TWNGUICustomType.NotThruCustom ? "1" : "2";
            }
            
            return new string(space, 1);
        }
        #endregion
    }

    #region Filter DAC
    [Serializable]
    [PXCacheName("TWN GUI Tran Filter")]
    public partial class GUITransFilter : PX.Data.IBqlTable
    {
        #region BranchID
        [Branch()]
        [PXDefault(typeof(AccessInfo.branchID))]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region FromDate
        [PXDBDateAndTime(UseTimeZone = true, PreserveTime = true)]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "From Date", Visibility = PXUIVisibility.Visible)]
        public virtual DateTime? FromDate { get; set; }
        public abstract class fromDate : IBqlField { }
        #endregion

        #region ToDate
        [PXDBDateAndTime(UseTimeZone = true, PreserveTime = true)]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "To Date", Visibility = PXUIVisibility.Visible)]
        public virtual DateTime? ToDate { get; set; }
        public abstract class toDate : IBqlField { }
        #endregion
    }
    #endregion
}