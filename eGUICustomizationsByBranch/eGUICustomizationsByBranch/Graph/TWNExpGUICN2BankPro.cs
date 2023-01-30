using System;
using System.Collections.Generic;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.GL;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;
using static eGUICustomizations.Graph.TWNExpGUIInv2BankPro;

namespace eGUICustomizations.Graph
{
    public class TWNExpGUICN2BankPro : PXGraph<TWNExpGUICN2BankPro>
    {
        #region String Constant Classes
        public class VATOutCode34 : PX.Data.BQL.BqlString.Constant<VATOutCode34>
        {
            public VATOutCode34() : base(TWGUIFormatCode.vATOutCode34) { }
        }
        #endregion

        #region Process & Setup
        public PXCancel<WHTTranFilter> Cancel;
        public PXFilter<WHTTranFilter> Filter;

        public PXFilteredProcessing<TWNGUITrans,
                                    WHTTranFilter,
                                    Where<TWNGUITrans.eGUIExcluded, Equal<False>,
                                          And2<Where<TWNGUITrans.eGUIExported, Equal<False>,
                                                     Or<TWNGUITrans.eGUIExported, IsNull>>,
                                              And<TWNGUITrans.gUIFormatCode, In3<ARRegisterExt.VATOut33Att, VATOutCode34>,
                                                  And<TWNGUITrans.branchID, Equal<Current<WHTTranFilter.branchID>>,
                                                      And<TWNGUITrans.allowUpload, Equal<True>>>>>>> GUITranProc;
        #endregion

        #region Ctor
        public TWNExpGUICN2BankPro()
        {
            GUITranProc.SetProcessCaption(ActionsMessages.Upload);
            GUITranProc.SetProcessAllCaption(TWMessages.UploadAll);
            GUITranProc.SetProcessDelegate(Upload);
        }
        #endregion

        #region Static Methods
        public static void Upload(List<TWNGUITrans> tWNGUITrans)
        {
            try
            {
                // Avoid to create empty content file in automation schedule.
                if (tWNGUITrans.Count == 0) { return; }

                TWNExpGUIInv2BankPro graph = CreateInstance<TWNExpGUIInv2BankPro>();

                string ourTaxNbrFixedBranch = BAccountExt.GetOurTaxNbByBranch(graph.GUITranProc.Cache, Branch.UK.Find(graph, fixedBranch)?.BranchID);

                string fileName = $"{ourTaxNbrFixedBranch}-AllowanceMD-{ourTaxNbrFixedBranch }-Paper-{DateTime.Today.ToString("yyyyMMdd")}-{DateTime.Now.ToString("hhmmss")}.txt";

                string lines = "";
                foreach (TWNGUITrans gUITrans in tWNGUITrans)
                {
                    #region M
                    // File Type
                    lines += "M" + verticalBar;
                    // Bill type
                    lines += TWNExpGUIInv2BankPro.GetBillType(gUITrans) + verticalBar;
                    // Invoice No
                    lines += verticalBar;
                    // Invoice Date Time
                    lines += verticalBar;
                    // Allowance Date
                    lines += gUITrans.GUIDate.Value.ToString("yyyyMMdd") + verticalBar;
                    // Cancel Date
                    lines += TWNExpGUIInv2BankPro.GetCancelDate(gUITrans) + verticalBar;
                    // Bill Attribute
                    lines += verticalBar;
                    // Seller Ban
                    lines += gUITrans.OurTaxNbr + verticalBar;
                    // Seller Code
                    lines += verticalBar;
                    // Buyer Ban
                    lines += gUITrans.TaxNbr + verticalBar;
                    // Buyer Code
                    lines += verticalBar;
                    // Buyer CName
                    lines += gUITrans.GUITitle + verticalBar;
                    // Sales Amount
                    lines += TWNExpGUIInv2BankPro.GetSalesAmt(gUITrans) + verticalBar;
                    // Tax Type
                    lines += TWNExpGUIInv2BankPro.GetTaxType(gUITrans.VATType) + verticalBar;
                    // Tax Rate
                    lines += TWNExpGUIInv2BankPro.GetTaxRate(gUITrans.VATType) + verticalBar;
                    // Tax Amount
                    lines += TWNExpGUIInv2BankPro.GetTaxAmt(gUITrans) + verticalBar;
                    // Total Amount
                    lines += (gUITrans.NetAmount + gUITrans.TaxAmount).Value + verticalBar;
                    // Health Tax
                    lines += "0" + verticalBar;
                    // Buyer Remark
                    lines += verticalBar;
                    // Main Remark
                    lines += verticalBar;
                    // Order No = Relate Number1
                    lines += (gUITrans.OrderNbr.Length > 16) ? gUITrans.OrderNbr.Substring(0, 16) : gUITrans.OrderNbr + verticalBar;
                    // Relate Number2                           
                    // Relate Number3
                    // Relate Number4
                    // Relate Number5
                    // Group Mark
                    // Customs Clearance Mark
                    lines += new string(char.Parse(verticalBar), 5) + TWNExpGUIInv2BankPro.GetCustomClearance(gUITrans) + verticalBar;
                    // Bonded Area Enum
                    lines += verticalBar;
                    // Random Number
                    lines += (gUITrans.BatchNbr != null) ? gUITrans.BatchNbr.Substring(0, 4) : null;
                    // Carrier Type
                    // Carrier ID
                    // NPOBAN
                    // Request Paper
                    // Void Reason
                    // Project Number Void Approved
                    lines += new string(char.Parse(verticalBar), 6) + "\r\n";
                    #endregion

                    #region D
                    // The following method is only for voided invoice.
                    if (gUITrans.GUIStatus == TWNStringList.TWNGUIStatus.Voided)
                    {
                        TWNExpGUIInv2BankPro.CreateVoidedDetailLine(verticalBar, gUITrans.OrderNbr, ref lines);
                    }
                    else
                    {
                        bool isB2C = string.IsNullOrEmpty(gUITrans.TaxNbr);
                        bool isInclusive = false;

                        PXResultset<ARTran> results = graph.RetrieveARTran(gUITrans.OrderNbr);

                        var validate = new TWNGUIValidation();

                        validate.CheckCorrespondingInv(graph, gUITrans.GUINbr, gUITrans.GUIFormatCode);

                        DateTime? origGUIDate = validate.tWNGUITrans?.GUIDate;

                        foreach (PXResult<ARTran> result in results)
                        {
                            ARTran aRTran = result;

                            // File Type
                            lines += "D" + verticalBar;
                            // Description
                            lines += aRTran.TranDesc + verticalBar;
                            // Quantity
                            lines += aRTran.Qty + verticalBar;
                            // Unit Price
                            // Amount
                            isInclusive = graph.AmountInclusiveTax(graph.GetInvoiceTaxCalcMode(graph, aRTran.TranType, aRTran.RefNbr), gUITrans.TaxID);

                            if (isB2C && isInclusive == false)
                            {
                                lines += aRTran.UnitPrice + verticalBar;
                                lines += aRTran.TranAmt + verticalBar;
                            }
                            else
                            {
                                lines += (aRTran.UnitPrice * TWNExpGUIInv2BankPro.fixedRate) + verticalBar;
                                lines += (aRTran.TranAmt * TWNExpGUIInv2BankPro.fixedRate) + verticalBar;
                            }
                            // Unit
                            lines += verticalBar;
                            // Package
                            lines += "0" + verticalBar;
                            // Gift Number 1 (Box)
                            lines += "0" + verticalBar;
                            // Gift Number 2 (Piece)
                            lines += "0" + verticalBar;
                            // Order No
                            lines += (gUITrans.OrderNbr.Length > 16) ? gUITrans.OrderNbr.Substring(0, 16) : gUITrans.OrderNbr + verticalBar;
                            // Buyer Barcode
                            // Buyer Prod No
                            // Seller Prod No
                            // Seller Account No
                            // Seller Shipping No
                            // Remark
                            // Relate Number1
                            // Relate Number2 (Invoice No)
                            lines += new string(char.Parse(verticalBar), 7) + gUITrans.GUINbr + verticalBar;
                            // Relate Number3 (Invoice Date)
                            // Relate Number4
                            // Relate Number5
                            lines += origGUIDate.Value.ToString("yyyy/MM/dd HH:mm:ss");
                            lines += new string(char.Parse(verticalBar), 2) + "\r\n";
                        }

                        if (results.Count <= 0)
                        {
                            isInclusive = graph.AmountInclusiveTax(PX.Objects.TX.TaxCalculationMode.TaxSetting, gUITrans.TaxID);

                            foreach (TWNGUIPrintedLineDet line in SelectFrom<TWNGUIPrintedLineDet>.Where<TWNGUIPrintedLineDet.gUINbr.IsEqual<@P.AsString>
                                                                                                         .And<TWNGUIPrintedLineDet.gUIFormatcode.IsEqual<@P.AsString>
                                                                                                              .And<TWNGUIPrintedLineDet.refNbr.IsEqual<@P.AsString>>>>
                                                                                                  .View.Select(graph, gUITrans.GUINbr, gUITrans.GUIFormatCode, gUITrans.OrderNbr))
                            {
                                (decimal UnitPrice, decimal ExtPrice) = CreateInstance<TWNExpOnlineStrGUIInv>().CalcTaxAmt(isInclusive,
                                                                                                                           !isB2C,
                                                                                                                           line.UnitPrice.Value,
                                                                                                                           line.Amount.Value);

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
                                lines += (gUITrans.OrderNbr.Length > 16) ? gUITrans.OrderNbr.Substring(0, 16) : gUITrans.OrderNbr + verticalBar;
                                // Buyer Barcode
                                // Buyer Prod No
                                // Seller Prod No
                                // Seller Account No
                                // Seller Shipping No
                                // Remark
                                // Relate Number1
                                // Relate Number2 (Invoice No)
                                lines += new string(char.Parse(verticalBar), 7) + gUITrans.GUINbr + verticalBar;
                                // Relate Number3 (Invoice Date)
                                // Relate Number4
                                // Relate Number5
                                lines += origGUIDate.Value.ToString("yyyy/MM/dd HH:mm:ss");
                                lines += new string(char.Parse(verticalBar), 2) + "\r\n";
                            }
                        }
                    }
                    #endregion
                }

                // Total Records
                lines += tWNGUITrans.Count;

                graph.UpdateGUITran(tWNGUITrans);
                graph.UploadFile2FTP(fileName, lines);
            }
            catch (Exception ex)
            {
                PXProcessing<TWNGUITrans>.SetError(ex);
                throw;
            }
        }
        #endregion
    }
}