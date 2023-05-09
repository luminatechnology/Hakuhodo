using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using PX.Archiver;
using PX.Data;
using PX.Objects.CR;
using eGUICustomizations.Descriptor;
using eGUICustomizations.DAC;

namespace eGUICustomizations.Graph
{
    public class TWNGenWHTFile_New : TWNGenWHTFile
    {
        #region Features
        public PXFilteredProcessing<TWNWHTTran, WHTTranFilter,
                                    Where<TWNWHTTran.paymDate, GreaterEqual<Current<WHTTranFilter.fromPaymDate>>,
                                          And<TWNWHTTran.paymDate, LessEqual<Current<WHTTranFilter.toPaymDate>>,
                                              And<TWNWHTTran.branchID, Equal<Current<WHTTranFilter.branchID>>>>>> WHTTranProcNew;
        #endregion

        #region Ctor
        public TWNGenWHTFile_New()
        {
            WHTTranProcNew.SetProcessCaption(ActionsMessages.Export);
            WHTTranProcNew.SetProcessAllCaption(TWMessages.ExportAll);
            ///<remarks> Because the file is downloaded using the throw exception, an error message may be displayed even if there is no error.///</remarks>
            // Acuminator disable once PX1008 LongOperationDelegateSynchronousExecution [Justification]
            WHTTranProcNew.SetProcessDelegate(Export);
        }
        #endregion

        #region Override Methods
        public override void Export(List<TWNWHTTran> wHTTranList)
        {
            //base.Export(wHTTranList);
            List<byte[]> files = new List<byte[]>();

            TWNGenGUIMediaFile mediaGraph = PXGraph.CreateInstance<TWNGenGUIMediaFile>();

            WHTTranFilter     filter = Filter.Current;
            TWNGUIPreferences gUIPreferences = GUISetup.Current;

            string headFileName, bodyFileName;
            string verticalBar = TWNExpGUIInv2BankPro.verticalBar;

            BAccountExt branchExt = BAccountExt.GetTWGUIByBranch(this.WHTTranProc.Cache, filter?.BranchID);

            #region HEAD
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(stream, Encoding.Unicode))
                {
                    headFileName = $"{branchExt.UsrOurTaxNbr}.{filter?.FromPaymDate.Value.Year - 1911}_HEAD.U8";

                    // 1
                    string lines = branchExt.UsrWHTTaxAuthority + verticalBar;
                    // 2
                    lines += verticalBar;
                    // 3
                    lines += branchExt.UsrOurTaxNbr + verticalBar;
                    // 4 
                    lines += "1" + verticalBar;
                    // 5
                    lines += branchExt.UsrCompanyName + verticalBar;
                    // 6 
                    lines += branchExt.UsrAddressLine + verticalBar;
                    // 7 
                    lines += branchExt.UsrPersonInCharge + verticalBar;
                    // 8
                    lines += verticalBar;
                    // 9
                    lines += branchExt.UsrCompanyPhone + verticalBar;
                    // 10 
                    lines += verticalBar;
                    // 11
                    lines += branchExt.UsrTaxRegistrationID + verticalBar;
                    // 12
                    lines += "Y" + verticalBar;
                    // 13~16
                    lines += new string(Convert.ToChar(verticalBar), 4);

                    sw.Write(lines);

                    sw.Close();

                    var headFile = new PX.SM.FileInfo(Guid.NewGuid(), headFileName, null, stream.ToArray());
                    
                    files.Add(headFile.BinData);
                }

                stream.Close();
            }
            #endregion

            #region BODY
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(stream, Encoding.Unicode))
                {
                    int count = 1;

                    bodyFileName = $"{branchExt.UsrOurTaxNbr}.{filter?.FromPaymDate.Value.Year - 1911}_BODY.U8";

                    foreach (TWNWHTTran row in wHTTranList)
                    {
                        // 稽徵機關代號
                        string lines = branchExt?.UsrWHTTaxAuthority + verticalBar;
                        // 流水號
                        lines += PX.Objects.CS.AutoNumberAttribute.GetNextNumber(WHTTranProc.Cache, row, gUIPreferences.WHTFileNumbering, Accessinfo.BusinessDate) + verticalBar;
                        // 扣繳單位或營利事業統一編號
                        lines += branchExt?.UsrOurTaxNbr + verticalBar;
                        // 所得註記
                        lines += verticalBar;
                        // 所得格式
                        lines += row.WHTFmtCode + verticalBar;
                        // 所得人統一編(證)號
                        lines += row.PersonalID + verticalBar;
                        // 證號別
                        lines += row.TypeOfIn + verticalBar;
                        // 扣繳憑單給付總額
                        lines += Math.Round(row.NetAmt.Value, 0, MidpointRounding.AwayFromZero) + verticalBar;
                        // 扣繳憑單扣繳稅額
                        lines += Math.Round((row.SecNHIAmt + row.WHTAmt).Value, 0, MidpointRounding.AwayFromZero) + verticalBar;
                        // 扣繳憑單給付淨額
                        lines += Math.Round((row.NetAmt - row.SecNHIAmt - row.WHTAmt).Value, 0, MidpointRounding.AwayFromZero) + verticalBar;
                        // 租賃房屋稅籍編號、執行業務者業別代號、所得人代號或帳號、外僑護照號碼
                        lines += GetFormatNbr(row) + verticalBar;
                        // Blank 1
                        lines += verticalBar;
                        // Blank 2
                        lines += verticalBar;
                        // 軟體註記
                        lines += "B" + verticalBar;
                        // 錯誤註記
                        lines += verticalBar;
                        // 所得給付年度
                        lines += mediaGraph.GetTWNDate(row.PaymDate.Value).Substring(0, 3) + verticalBar;
                        // 所得人姓名/名稱
                        lines += row.PayeeName + verticalBar;
                        // 所得人地址
                        lines += row.PayeeAddr + verticalBar;
                        // 所得所屬期間
                        // The format is YYYMMYYYMM (From payment date and To payment date), the Chinese year and month are retrieved from the payment date entered in the selection criteria. 
                        lines += mediaGraph.GetTWNDate(row.PaymDate.Value) + mediaGraph.GetTWNDate(row.PaymDate.Value) + verticalBar;
                        // 依勞退條例或教職員退撫條例自願提繳之退休金額
                        // Blank 3
                        // Blank 4
                        // Blank 5
                        // Blank 6
                        // 扣抵稅額註記
                        // 憑單填發方式
                        lines += new string(Convert.ToChar(verticalBar), 7);
                        // 是否滿183天
                        lines += "Y" + verticalBar;
                        // Country
                        lines += row.CountryID + verticalBar;
                        // 租稅協定代碼
                        lines += verticalBar;
                        // Blank 7
                        lines += verticalBar;
                        // 檔案製作日期\非居住者所得給付月日
                        lines += this.Accessinfo.BusinessDate.Value.ToString("MMdd") + verticalBar;

                        // Only the last line does not need to be broken.
                        if (count < wHTTranList.Count)
                        {
                            sw.WriteLine(lines);
                            count++;
                        }
                        else
                        {
                            sw.Write(lines);
                        }
                    }

                    sw.Close();

                    var bodyFile = new PX.SM.FileInfo(Guid.NewGuid(), bodyFileName, null, stream.ToArray());

                    files.Add(bodyFile.BinData);
                }

                stream.Close();
            }
            #endregion   

            using (MemoryStream ms = new MemoryStream())
            {
                //ms.Seek(0, SeekOrigin.Begin);
                for (int i = 0; i < files.Count; i++)
                {
                    using (var zipArchive = new ZipArchiveWrapper(ms))
                    {
                        zipArchive[i == 0 ? headFileName : bodyFileName] = files[i];
                    }
                }

                throw new PXRedirectToFileException(new PX.SM.FileInfo($"{headFileName.Substring(0, headFileName.IndexOf('_'))}.zip", null, ms.ToArray()), true);
            }
        }
        #endregion
    }
}