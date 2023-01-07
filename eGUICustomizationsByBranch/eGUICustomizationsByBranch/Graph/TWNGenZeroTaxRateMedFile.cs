using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;

namespace eGUICustomizations.Graph
{
    public class TWNGenZeroTaxRateMedFile : PXGraph<TWNGenZeroTaxRateMedFile>
    {
        #region Features & Setup
        public PXCancel<GUITransFilter> Cancel;
        public PXFilter<GUITransFilter> FilterGUITran;
        public PXFilteredProcessing<TWNGUITrans, GUITransFilter,
                                    Where<TWNGUITrans.gUIDecPeriod, Between<Current<GUITransFilter.fromDate>, Current<GUITransFilter.toDate>>,
                                          And<TWNGUITrans.vATType, Equal<TWNStringList.TWNGUIVATType.zero>,
                                              And<TWNGUITrans.gUIStatus, NotEqual<TWNStringList.TWNGUIStatus.voided>,
                                                  And<TWNGUITrans.gUIFormatCode, NotIn3<ARRegisterExt.VATOut34Att, 
                                                                                        ARRegisterExt.VATOut33Att>>>>>> GUITranProc;
        #endregion

        #region Ctor
        public TWNGenZeroTaxRateMedFile()
        {
            GUITranProc.SetProcessCaption(ActionsMessages.Export);
            GUITranProc.SetProcessAllCaption(TWMessages.ExportAll);
            GUITranProc.SetProcessDelegate(Export);

            //GUITranProc.SetProcessDelegate<TWNGenZeroTaxRateMedFile>(delegate (TWNGenZeroTaxRateMedFile graph, TWNGUITrans order)
            //{
            //    try
            //    {
            //        graph.Clear();
            //        graph.Export(order);
            //    }
            //    catch (Exception e)
            //    {
            //        PXProcessing<TWNGUITrans>.SetError(e);
            //    }
            //});
        }
        #endregion

        #region Event Handler
        protected void _(Events.FieldUpdated<GUITransFilter.toDate> e)
        {
            e.Cache.SetValue<GUITransFilter.toDate>(e.Row, DateTime.Parse(e.NewValue.ToString()).AddDays(1).Date.AddSeconds(-1));
        }
        #endregion

        #region Methods
        public void Export(List<TWNGUITrans> tWNGUITrans)
        {
            try
            {
                TWNGenGUIMediaFile mediaFile = PXGraph.CreateInstance<TWNGenGUIMediaFile>();

                int    count = 1;
                string lines = "", ticketType = "X7";

                using (MemoryStream ms = new MemoryStream())
                {
                    using (StreamWriter sw = new StreamWriter(ms, Encoding.ASCII))
                    {
                        string fileName = $"{BAccountExt.GetOurTaxNbByBranch(mediaFile.GUITransList.Cache, tWNGUITrans[0].BranchID)}.t02";

                        foreach (TWNGUITrans gUITrans in tWNGUITrans)
                        {
                            BAccountExt branchExt = BAccountExt.GetTWGUIByBranch(mediaFile.GUITransList.Cache, gUITrans.BranchID);

                            // Tax ID
                            lines = branchExt?.UsrOurTaxNbr;
                            // Country No
                            lines += branchExt?.UsrZeroTaxTaxCntry;
                            // Tax Registration ID
                            lines += branchExt?.UsrTaxRegistrationID;
                            // Tax Filling Date
                            lines += mediaFile.GetTWNDate(FilterGUITran.Current.ToDate.Value);
                            // GUI Date
                            lines += mediaFile.GetTWNDate(gUITrans.GUIDate.Value);
                            // GUI Number
                            lines += mediaFile.GetGUINbr(gUITrans);
                            // Customer Tax ID
                            lines += gUITrans.TaxNbr ?? new string(mediaFile.space, 8);
                            // Export Method
                            lines += gUITrans.ExportMethods;                               
                            // Custom Method
                            lines += gUITrans.CustomType;
                            // Ticket Type
                            // Ticket Number
                            if (gUITrans.ExportTicketType == ticketType)
                            {
                                lines += new String(mediaFile.space, 2);
                                lines += new String(mediaFile.space, 14);
                            }
                            else
                            {
                                lines += gUITrans.ExportTicketType;
                                lines += gUITrans.ExportTicketNbr;
                            }
                            // Amount
                            lines += mediaFile.GetNetAmt(gUITrans);
                            // Custom Clearing Date
                            lines += mediaFile.GetTWNDate(gUITrans.ClearingDate.Value, true);

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

                        //Write to file
                        //FixedLengthFile flatFile = new FixedLengthFile();
                        //flatFile.WriteToFile(recordList, sw);
                        //sw.Flush();

                        sw.Close();

                        PX.SM.FileInfo info = new PX.SM.FileInfo(fileName, null, ms.ToArray());

                        throw new PXRedirectToFileException(info, true);
                    }
                }
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