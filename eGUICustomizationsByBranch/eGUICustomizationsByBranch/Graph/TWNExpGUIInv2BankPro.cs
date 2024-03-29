﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.GL;
using PX.Objects.TX;
using PX.Objects.SO;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;

namespace eGUICustomizations.Graph
{
    public class TWNExpGUIInv2BankPro : PXGraph<TWNExpGUIInv2BankPro>
    {
        public const decimal fixedRate = 1.05m;
        public const string  verticalBar = "|";
        public const string  fixedBranch = "P-PILOT";

        #region String Constant Classes
        public class VATOutCode31 : PX.Data.BQL.BqlString.Constant<VATOutCode31>
        {
            public VATOutCode31() : base(TWGUIFormatCode.vATOutCode31) { }
        }

        public class VATOutCode32 : PX.Data.BQL.BqlString.Constant<VATOutCode32>
        {
            public VATOutCode32() : base(TWGUIFormatCode.vATOutCode32) { }
        }

        public class VATOutCode35 : PX.Data.BQL.BqlString.Constant<VATOutCode35>
        {
            public VATOutCode35() : base(TWGUIFormatCode.vATOutCode35) { }
        }
        #endregion

        #region Features & Setup
        public PXCancel<WHTTranFilter> Cancel;
        public PXFilter<WHTTranFilter> Filter;

        public PXFilteredProcessing<TWNGUITrans, 
                                    WHTTranFilter,
                                    Where<TWNGUITrans.eGUIExcluded, Equal<False>,
                                          And2<Where<TWNGUITrans.eGUIExported, Equal<False>,
                                                     Or<TWNGUITrans.eGUIExported, IsNull>>,
                                              And<TWNGUITrans.gUIFormatCode, Equal<VATOutCode35>,
                                                  And<TWNGUITrans.branchID, Equal<Current<WHTTranFilter.branchID>>,
                                                      And<TWNGUITrans.allowUpload, Equal<True>>>>>>> GUITranProc;

        public PXSetup<TWNGUIPreferences> gUIPreferSetup;
        #endregion

        #region Ctor
        public TWNExpGUIInv2BankPro()
        {
            GUITranProc.SetProcessCaption(ActionsMessages.Upload);
            GUITranProc.SetProcessAllCaption(TWMessages.UploadAll);
            GUITranProc.SetProcessDelegate(Upload);
        }
        #endregion

        #region Methods
        public void UpdateGUITran(List<TWNGUITrans> tWNGUITrans)
        {
            foreach (TWNGUITrans trans in tWNGUITrans)
            {
                trans.EGUIExported = true;
                trans.EGUIExportedDateTime = DateTime.UtcNow;

                GUITranProc.Cache.Update(trans);
            }

            this.Actions.PressSave();
        }

        public void UploadFile2FTP(string fileName, string content)
        {
            ///<remarks>
            ///ftpes:// is not a standard protocol prefix. But it is recognized by some FTP clients to mean "Explicit FTP over TLS/SSL".
            ///With FtpWebRequest you specify that by using standard ftp:// protocol prefix and setting EnableSsl = true
            ///</remarks>
            string uRL = gUIPreferSetup.Current.Url.Replace("ftps", "ftp");

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(uRL + fileName));

            request.Credentials = new NetworkCredential(gUIPreferSetup.Current.UserName, gUIPreferSetup.Current.Password);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            //request.EnableSsl = true;

            byte[] data = System.Text.Encoding.UTF8.GetBytes(content);

            request.ContentLength = data.Length;

            Stream requestStream = request.GetRequestStream();

            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            requestStream.Dispose();

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                response.Close();
                /// Close FTP
                request.Abort();

                //throw new PXOperationCompletedException(message);
            }
        }

        public virtual string GetInvoiceTaxCalcMode(PXGraph graph, string tranType, string refNbr)
        {
            return SelectFrom<ARRegister>.Where<ARRegister.docType.IsEqual<@P.AsString>
                                                .And<ARRegister.refNbr.IsEqual<@P.AsString>>>
                                         .View.ReadOnly.Select(graph, tranType, refNbr).TopFirst.TaxCalcMode;
        }

        public bool AmountInclusiveTax(string taxCalcMode, string taxID)
        {
            bool value;
            switch (taxCalcMode)
            {
                case TaxCalculationMode.Gross:
                    value = true;
                    break;
                case TaxCalculationMode.Net:
                    value = false;
                    break;
                case TaxCalculationMode.TaxSetting:
                    value = Tax.PK.Find(this, taxID).TaxCalcLevel == CSTaxCalcLevel.Inclusive;
                    break;
                default:
                    value = false;
                    break;
            }

            return value;
        }
        #endregion

        #region Static Methods
        public static void Upload(List<TWNGUITrans> tWNGUITrans)
        {
            //try
            //{
                // Avoid to create empty content file in automation schedule.
                if (tWNGUITrans.Count == 0) { return; }

                TWNExpGUIInv2BankPro graph = CreateInstance<TWNExpGUIInv2BankPro>();

                string ourTaxNbrFixedBranch = BAccountExt.GetOurTaxNbByBranch(graph.GUITranProc.Cache, Branch.UK.Find(graph, fixedBranch)?.BranchID);

                string fileName = $"{ourTaxNbrFixedBranch}-InvoiceMD-{ourTaxNbrFixedBranch}-Paper-{DateTime.Today:yyyMMdd}-{DateTime.Now:hhmmss}.txt";

                string lines = "";
                foreach (TWNGUITrans gUITrans in tWNGUITrans)
                {
                    #region M
                    // File Type
                    lines += "M" + verticalBar;
                    // Bill type
                    lines += GetBillType(gUITrans) + verticalBar;
                    // Invoice No
                    lines += gUITrans.GUINbr + verticalBar;
                    // Invoice Date Time
                    lines += gUITrans.GUIDate.Value.ToString("yyyy/MM/dd HH:mm:ss") + verticalBar;
                    // Allowance Date
                    // Cancel Date
                    lines += verticalBar + GetCancelDate(gUITrans) + verticalBar;
                    // Bill Attribute
                    // Seller Ban
                    lines += verticalBar + gUITrans.OurTaxNbr + verticalBar;
                    // Seller Code
                    lines += verticalBar;
                    // Buyer Ban
                    lines += gUITrans.TaxNbr + verticalBar;
                    // Buyer Code
                    lines += verticalBar;
                    // Buyer CName
                    lines += gUITrans.GUITitle + verticalBar;
                    // Sales Amount
                    lines += GetSalesAmt(gUITrans) + verticalBar;
                    // Tax Type
                    lines += GetTaxType(gUITrans.VATType) + verticalBar;
                    // Tax Rate
                    lines += GetTaxRate(gUITrans.VATType) + verticalBar;
                    // Tax Amount
                    lines += GetTaxAmt(gUITrans) + verticalBar;
                    // Total Amount
                    lines += (gUITrans.NetAmount + gUITrans.TaxAmount).Value + verticalBar;
                    // Health Tax
                    lines += "0" + verticalBar;
                    // Buyer Remark
                    lines += verticalBar;
                    // Main Remark
                    lines += verticalBar;
                    // Order No = Relate Number1
                    lines += gUITrans.OrderNbr + verticalBar;
                    // Relate Number2
                    // Relate Number3
                    // Relate Number4
                    // Relate Number5
                    // Group Mark
                    // Customs Clearance Mark
                    lines += new string(char.Parse(verticalBar), 5) + GetCustomClearance(gUITrans) + verticalBar;
                    // Bonded Area Enum
                    lines += verticalBar;
                    // Random Number
                    lines += (gUITrans.BatchNbr != null) ? gUITrans.BatchNbr.Substring(gUITrans.BatchNbr.Length - 4, 4) : null;
                    lines += verticalBar;
                    // Carrier Type
                    Tuple<string, string, string> tuple = new ARReleaseProcess_Extension().GetB2CTypeValue(string.IsNullOrEmpty(gUITrans.TaxNbr), gUITrans.CarrierID, gUITrans.NPONbr);
                    lines += tuple.Item1 + verticalBar;
                    // Carrier ID
                    lines += tuple.Item2 + verticalBar;
                    // NPOBAN
                    lines += tuple.Item3 + verticalBar;
                    // Request Paper
                    lines += gUITrans.B2CPrinted == true ? "Y" : "N" + verticalBar;
                    // Void Reason
                    // Project Number Void Approved
                    lines += new string(char.Parse(verticalBar), 2) + "\r\n";
                    #endregion

                    #region D
                    if (gUITrans.GUIStatus == TWNStringList.TWNGUIStatus.Voided)
                    {
                        CreateVoidedDetailLine(verticalBar, gUITrans.OrderNbr, ref lines);
                    }
                    else
                    {
                        bool isB2C       = string.IsNullOrEmpty(gUITrans.TaxNbr);
                        bool isInclusive = graph.AmountInclusiveTax(TaxCalculationMode.TaxSetting, gUITrans.TaxID);

                        PXResultset<TWNGUIPrintedLineDet> results = SelectFrom<TWNGUIPrintedLineDet>.Where<TWNGUIPrintedLineDet.gUINbr.IsEqual<@P.AsString>
                                                                                                           .And<TWNGUIPrintedLineDet.gUIFormatcode.IsEqual<@P.AsString>
                                                                                                                .And<TWNGUIPrintedLineDet.refNbr.IsEqual<@P.AsString>>>>
                                                                                                    .View.Select(graph, gUITrans.GUINbr, gUITrans.GUIFormatCode, gUITrans.OrderNbr);
                        foreach (var row in results)
                        {
                            TWNGUIPrintedLineDet line = row;

                            (decimal UnitPrice, decimal ExtPrice) = (line.UnitPrice.Value, line.Amount.Value);

                            // File Type
                            lines += "D" + verticalBar;
                            // Description
                            lines += line.Descr + verticalBar;
                            // Quantity
                            lines += (line.Qty ?? 1) + verticalBar;
                            // Unit Price
                            lines += string.Format("{0:0.####}", UnitPrice) + verticalBar;
                            // Amount
                            lines += string.Format("{0:0.####}", ExtPrice) + verticalBar;
                            // Unit
                            lines += verticalBar;
                            // Package
                            lines += "0" + verticalBar;
                            // Gift Number 1 (Box)
                            lines += "0" + verticalBar;
                            // Gift Number 2 (Piece)
                            lines += "0" + verticalBar;
                            // Order No
                            lines += gUITrans.OrderNbr;
                            // Buyer Barcode
                            // Buyer Prod No
                            // Seller Prod No
                            // Seller Account No
                            // Seller Shipping No
                            // Remark
                            // Relate Number1
                            // Relate Number2 (Invoice No)
                            // Relate Number3 (Invoice Date)
                            // Relate Number4
                            // Relate Number5
                            lines += new string(char.Parse(verticalBar), 11) + "\r\n";
                        }

                        if (results.Count <= 0)
                        {
                            foreach (ARTran aRTran in graph.RetrieveARTran(gUITrans.OrderNbr))
                            {
                                //ARTran aRTran = result;

                                // File Type
                                lines += "D" + verticalBar;
                                // Description
                                lines += aRTran.TranDesc + verticalBar;
                                // Quantity
                                lines += (aRTran.Qty ?? 1) + verticalBar;
                                // Unit Price
                                // Amount
                                #region Convert design spec logic to code.
                                //if (aRTran.CuryDiscAmt == 0m)
                                //{
                                //    if (taxCalcMode != PX.Objects.TX.TaxCalculationMode.Gross)
                                //    {
                                //        if (!string.IsNullOrEmpty(gUITrans.TaxNbr))
                                //        {
                                //            lines += aRTran.UnitPrice + verticalBar;
                                //        }
                                //        else
                                //        {
                                //            lines += aRTran.UnitPrice * fixedRate + verticalBar;
                                //        }
                                //    }
                                //    else
                                //    {
                                //        if (!string.IsNullOrEmpty(gUITrans.TaxNbr))
                                //        {
                                //            lines += aRTran.UnitPrice / fixedRate + verticalBar;
                                //        }
                                //        else
                                //        {
                                //            lines += aRTran.UnitPrice + verticalBar;
                                //        }
                                //    }
                                //}
                                //else
                                //{
                                //    if (taxCalcMode != PX.Objects.TX.TaxCalculationMode.Gross)
                                //    {
                                //        if (!string.IsNullOrEmpty(gUITrans.TaxNbr))
                                //        {
                                //            lines += aRTran.TranAmt / aRTran.Qty + verticalBar;
                                //        }
                                //        else
                                //        {
                                //            lines += aRTran.TranAmt / aRTran.Qty * fixedRate + verticalBar;
                                //        }
                                //    }
                                //    else
                                //    {
                                //        if (!string.IsNullOrEmpty(gUITrans.TaxNbr))
                                //        {
                                //            lines += aRTran.TranAmt / aRTran.Qty / fixedRate + verticalBar;
                                //        }
                                //        else
                                //        {
                                //            lines += aRTran.TranAmt / aRTran.Qty + verticalBar;
                                //        }
                                //    }
                                //}
                                #endregion

                                decimal? unitPrice = (aRTran.CuryDiscAmt == 0m) ? aRTran.UnitPrice : (aRTran.TranAmt / aRTran.Qty);
                                decimal? tranAmt = aRTran.TranAmt;

                                isInclusive = graph.AmountInclusiveTax(graph.GetInvoiceTaxCalcMode(graph, aRTran.TranType, aRTran.RefNbr), gUITrans.TaxID);

                                if (isB2C && isInclusive == false)
                                {
                                    unitPrice *= fixedRate;
                                    tranAmt *= fixedRate;
                                }
                                else if (!isB2C && isInclusive == true)
                                {
                                    unitPrice /= fixedRate;
                                    tranAmt /= fixedRate;
                                }
                                lines += string.Format("{0:0.####}", unitPrice) + verticalBar;
                                lines += string.Format("{0:0.####}", tranAmt) + verticalBar;
                                // Unit
                                lines += verticalBar;
                                // Package
                                lines += "0" + verticalBar;
                                // Gift Number 1 (Box)
                                lines += "0" + verticalBar;
                                // Gift Number 2 (Piece)
                                lines += "0" + verticalBar;
                                // Order No
                                lines += gUITrans.OrderNbr;
                                // Buyer Barcode
                                // Buyer Prod No
                                // Seller Prod No
                                // Seller Account No
                                // Seller Shipping No
                                // Remark
                                // Relate Number1
                                // Relate Number2 (Invoice No)
                                // Relate Number3 (Invoice Date)
                                // Relate Number4
                                // Relate Number5
                                lines += new string(char.Parse(verticalBar), 11) + "\r\n";
                            }
                        }
                    }
                    #endregion
                }

            // Total Records
            lines += tWNGUITrans.Count;

            graph.UpdateGUITran(tWNGUITrans);
                graph.UploadFile2FTP(fileName, lines);
            //}
            //catch (Exception ex)
            //{
            //    PXProcessing<TWNGUITrans>.SetError(ex);
            //    throw;
            //}
        }

        public static string GetBillType(TWNGUITrans gUITran)
        {
            string billType = null;

            switch (gUITran.GUIFormatCode)
            {
                case TWGUIFormatCode.vATOutCode35:
                case TWGUIFormatCode.vATOutCode31:
                    if (gUITran.GUIStatus == TWNStringList.TWNGUIStatus.Used) { billType = "O"; }
                    else if (gUITran.GUIStatus == TWNStringList.TWNGUIStatus.Voided) { billType = "C"; }
                    break;

                case TWGUIFormatCode.vATOutCode33:
                    if (gUITran.GUIStatus == TWNStringList.TWNGUIStatus.Used) { billType = "A2"; }
                    else if (gUITran.GUIStatus == TWNStringList.TWNGUIStatus.Voided) { billType = "D"; }
                    break;
            }

            return billType;
        }

        public static string GetTaxType(string vATType)
        {
            if (vATType == TWNStringList.TWNGUIVATType.Five) { return "1"; }
            else if (vATType == TWNStringList.TWNGUIVATType.Zero) { return "2"; }
            else { return "3"; }
        }

        public static string GetTaxRate(string vATType)
        {
            return (vATType == TWNStringList.TWNGUIVATType.Five) ? "0.05" : "0";
        }

        public static string GetCustomClearance(TWNGUITrans gUITran)
        {
            if (gUITran.CustomType == TWNStringList.TWNGUICustomType.NotThruCustom &&
                gUITran.VATType == TWNStringList.TWNGUIVATType.Five)
            {
                return "1";
            }
            if (gUITran.CustomType == TWNStringList.TWNGUICustomType.ThruCustom &&
                gUITran.VATType == TWNStringList.TWNGUIVATType.Zero)
            {
                return "2";
            }
            else
            {
                return null;
            }
        }

        public static string GetCancelDate(TWNGUITrans gUITran)
        {
            return gUITran.GUIStatus ==TWNStringList.TWNGUIStatus.Voided ? gUITran.GUIDate.Value.ToString("yyyyMMdd") : string.Empty;
        }

        public static decimal GetSalesAmt(TWNGUITrans gUITran)
        {
            return string.IsNullOrEmpty(gUITran.TaxNbr) ? (gUITran.NetAmount + gUITran.TaxAmount).Value : gUITran.NetAmount.Value;
        }

        public static decimal GetTaxAmt(TWNGUITrans gUITran)
        {
            return string.IsNullOrEmpty(gUITran.TaxNbr) ? 0 : gUITran.TaxAmount.Value;
        }

        public static void CreateVoidedDetailLine(string verticalBar, string refNbr, ref string lines)
        {
            // File Type
            lines += "D" + verticalBar;
            // Description
            lines += "Service" + verticalBar;
            // Quantity
            lines += "0" + verticalBar;
            // Unit Price
            lines += "0" + verticalBar;
            // Amount
            lines += "0" + verticalBar;
            // Unit
            lines += verticalBar;
            // Package
            lines += "0" + verticalBar;
            // Gift Number 1 (Box)
            lines += "0" + verticalBar;
            // Gift Number 2 (Piece)
            lines += "0" + verticalBar;
            // Order No
            lines += refNbr;
            // Buyer Barcode
            // Buyer Prod No
            // Seller Prod No
            // Seller Account No
            // Seller Shipping No
            // Remark
            // Relate Number1
            // Relate Number2 (Invoice No)
            // Relate Number3 (Invoice Date)
            // Relate Number4
            // Relate Number5
            lines += new string(char.Parse(verticalBar), 11) + "\r\n";
        }
        #endregion

        #region Search Result
        public PXResultset<ARTran> RetrieveARTran(string orderNbr)
        {
            return SelectFrom<ARTran>.Where<ARTran.refNbr.IsEqual<@P.AsString>
                                            .And<Where<ARTran.lineType.IsNotEqual<SOLineType.discount>
                                                       .Or<ARTran.lineType.IsNull>>>>.View.ReadOnly.Select(this, orderNbr);
        }
        #endregion
    }
}