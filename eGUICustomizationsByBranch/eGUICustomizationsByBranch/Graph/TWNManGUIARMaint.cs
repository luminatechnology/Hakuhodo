using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.TX;
using PX.Objects.SO;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;
using eGUICustomizations.Graph_Release;

namespace eGUICustomizations.Graph
{
    public class TWNManGUIAREntry : PXGraph<TWNManGUIAREntry>
    {
        public TWNReleaseProcess rp = PXGraph.CreateInstance<TWNReleaseProcess>();

        #region Selects & Setup
        public PXSave<TWNGUIManualFilter> Save;

        public PXCancel<TWNGUIManualFilter> Cancel;

        public PXFilter<TWNGUIManualFilter> Filter;

        [PXImport(typeof(TWNManualGUIAR))]
        [PXFilterable()]
        public SelectFrom<TWNManualGUIAR>.Where<TWNManualGUIAR.status.IsEqual<TWNGUIManualFilter.status.FromCurrent>>.View ManualGUIAR;

        //public SelectFrom<TWNGUITrans>.Where<TWNGUITrans.gUINbr.IsEqual<TWNManualGUIAR.gUINbr.FromCurrent>>.View ViewGUITrans;

        public PXSetup<TWNGUIPreferences> GUIPreferences;
        #endregion

        #region Overrie Methods
        public override void Persist()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                foreach (TWNManualGUIAR row in ManualGUIAR.Cache.Deleted)
                {
                    if (tWNGUIValidation.isCreditNote == false && !string.IsNullOrEmpty(row.GUINbr))
                    {
                        InsertGUITran(row, BAccount.PK.Find(this, row.CustomerID), true);
                    }
                }

                base.Persist();

                ts.Complete();
            }
        }
        #endregion

        #region Actions
        public PXAction<TWNGUIManualFilter> release;
        [PXProcessButton(ImageSet = "main", ImageKey = PX.Web.UI.Sprite.Main.Release), PXUIField(DisplayName = ActionsMessages.Release)]
        public virtual IEnumerable Release(PXAdapter adapter)
        {
            TWNManualGUIAR manualGUIAR = this.ManualGUIAR.Current;

            if (manualGUIAR == null || string.IsNullOrEmpty(manualGUIAR.GUINbr))
            {
                throw new PXException(TWMessages.GUINbrIsMandat);
            }
            else
            {
                PXLongOperation.StartOperation(this, () =>
                {
                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        TWNGUITrans tWNGUITrans = rp.InitAndCheckOnAR(manualGUIAR.GUINbr, manualGUIAR.VatOutCode);

                        InsertGUITran(manualGUIAR, BAccount.PK.Find(this, manualGUIAR.CustomerID), false);

                        manualGUIAR.Status = TWNStringList.TWNGUIManualStatus.Released;
                        ManualGUIAR.Cache.MarkUpdated(manualGUIAR);

                        Actions.PressSave();

                        if (tWNGUITrans != null)
                        {
                            rp.ViewGUITrans.SetValueExt<TWNGUITrans.netAmtRemain>(tWNGUITrans, (tWNGUITrans.NetAmtRemain -= manualGUIAR.NetAmt));
                            rp.ViewGUITrans.SetValueExt<TWNGUITrans.taxAmtRemain>(tWNGUITrans, (tWNGUITrans.TaxAmtRemain -= manualGUIAR.TaxAmt));
                            rp.ViewGUITrans.Update(tWNGUITrans);

                            rp.ViewGUITrans.Cache.Persist(PXDBOperation.Update);
                            rp.ViewGUITrans.Cache.Persisted(false);
                        }

                        ts.Complete();
                    }
                });
            }

            return adapter.Get();
        }

        public PXAction<TWNGUIManualFilter> printGUIInvoice;
        [PXButton(ImageSet = "main", ImageKey = PX.Web.UI.Sprite.Main.ReportF), PXUIField(DisplayName = "Print GUI Invoice")]
        public virtual IEnumerable PrintGUIInvoice(PXAdapter adapter)
        {
            var current = ManualGUIAR.Current;

            if (current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    [nameof(TWNGUITrans.GUIFormatCode)] = current.VatOutCode,
                    [nameof(ARInvoice.RefNbr)]          = current.GUINbr,
                    [nameof(TWNGUITrans.GUINbr)]        = current.GUINbr
                };

                throw new PXReportRequiredException(parameters, SOInvoiceEntry_Extension.GUIReportID, SOInvoiceEntry_Extension.GUIReportID);
            }

            return adapter.Get();
        }
        #endregion

        #region Event Handlers
        TWNGUIValidation tWNGUIValidation = new TWNGUIValidation();

        protected virtual void _(Events.RowSelected<TWNGUIManualFilter> e)
        {
            bool isOpen = e.Row.Status == TWNStringList.TWNGUIManualStatus.Open;

            ManualGUIAR.Cache.AllowDelete = ManualGUIAR.Cache.AllowInsert = ManualGUIAR.Cache.AllowUpdate = isOpen;
            release.SetEnabled(isOpen);
            printGUIInvoice.SetEnabled(!isOpen);
        }

        protected virtual void _(Events.RowPersisting<TWNManualGUIAR> e)
        {
            var gUIPref = GUIPreferences.Current;

            AutoNumberAttribute.SetNumberingId<TWNManualGUIAR.gUINbr>(e.Cache, e.Row.VatOutCode == TWGUIFormatCode.vATOutCode31 ? gUIPref.GUI3CopiesManNumbering :
                                                                                                                                  e.Row.VatOutCode == TWGUIFormatCode.vATOutCode32 ? gUIPref.GUI2CopiesNumbering : 
                                                                                                                                                                                     gUIPref.GUI3CopiesNumbering);
        }

        protected virtual void _(Events.FieldUpdated<TWNManualGUIAR.customerID> e)
        {
            var row = (TWNManualGUIAR)e.Row;

            PXResult<Location> result = SelectFrom<Location>.InnerJoin<Customer>.On<Location.bAccountID.IsEqual<Customer.bAccountID>
                                                                                    .And<Location.locationID.IsEqual<Customer.defLocationID>>>
                                                            .InnerJoin<TaxZone>.On<TaxZone.taxZoneID.IsEqual<Location.cTaxZoneID>>
                                                            .InnerJoin<TaxZoneDet>.On<TaxZoneDet.taxZoneID.IsEqual<Location.cTaxZoneID>>
                                                            .Where<Customer.bAccountID.IsEqual<@P.AsInt>>.View.Select(this, row.CustomerID);
            if (result != null)
            {
                row.TaxZoneID = result.GetItem<Location>().CTaxZoneID;
                row.TaxCategoryID = result.GetItem<TaxZone>().DfltTaxCategoryID;
                row.TaxID = result.GetItem<TaxZoneDet>().TaxID;

                foreach (CSAnswers cS in SelectFrom<CSAnswers>.Where<CSAnswers.refNoteID.IsEqual<@P.AsGuid>>.View.ReadOnly.Select(this, result.GetItem<Customer>().NoteID))
                {
                    switch (cS.AttributeID)
                    {
                        case ARRegisterExt.VATOUTFRMTName:
                            row.VatOutCode = cS.Value;
                            break;

                        case ARRegisterExt.OurTaxNbrName:
                            row.OurTaxNbr = cS.Value;
                            break;

                        case ARRegisterExt.TaxNbrName:
                            row.TaxNbr = cS.Value;
                            break;
                    }
                }
            }
        }

        protected virtual void _(Events.FieldUpdated<TWNManualGUIAR.branchID> e)
        {
            var row = (TWNManualGUIAR)e.Row;

            row.OurTaxNbr = BAccountExt.GetOurTaxNbByBranch(e.Cache, (int?)e.NewValue);
        }
        #endregion

        #region Methods
        private void InsertGUITran(TWNManualGUIAR data, BAccount bAccount, bool isDeleted)
        {
            rp.CreateGUITrans(new STWNGUITran()
            {
                VATCode       = data.VatOutCode,
                GUINbr        = data.GUINbr,
                GUIStatus     = isDeleted == false ? TWNStringList.TWNGUIStatus.Used : TWNStringList.TWNGUIStatus.Voided,
                BranchID      = data.BranchID,
                GUIDirection  = TWNStringList.TWNGUIDirection.Issue,
                GUIDate       = data.GUIDate,
                GUITitle      = data.GUITitle,//bAccount?.AcctName,
                TaxZoneID     = data.TaxZoneID,
                TaxCategoryID = data.TaxCategoryID,
                TaxID         = data.TaxID,
                TaxNbr        = data.TaxNbr,
                OurTaxNbr     = data.OurTaxNbr,
                NetAmount     = isDeleted == false ? data.NetAmt : 0m,
                TaxAmount     = isDeleted == false ? data.TaxAmt : 0m,
                AcctCD        = bAccount?.AcctCD,
                AcctName      = bAccount?.AcctName,
                eGUIExcluded  = data.VatOutCode == TWGUIFormatCode.vATOutCode32,
                Remark        = isDeleted == false ? data.Remark : string.Format(TWMessages.DeleteInfo, this.Accessinfo.UserName),
                OrderNbr      = data.GUINbr,
                AddressLine   = data.AddressLine
            });

            #region InsertPrintedLine
            if (isDeleted == false)
            {
                decimal? amount = string.IsNullOrEmpty(data.TaxNbr) ? data.NetAmt + data.TaxAmt : data.NetAmt;

                List<(string, int, string, decimal?, decimal?, decimal?, string)> list = new List<ValueTuple<string, int, string, decimal?, decimal?, decimal?, string>>();

                list.Add(ValueTuple.Create(data.GUINbr, 1, CSAnswers.PK.Find(this, bAccount?.NoteID, "REMITDESC")?.Value, 1, amount, amount, $"{data.VatOutCode}-{data.GUINbr}-{data.Remark}"));

                rp.GeneratePrintedLineDetails(list);
            }
            #endregion
        }
        #endregion
    }

    #region Unbound DAC
    [PXCacheName("Filter Manual Trans")]
    public partial class TWNGUIManualFilter : GUITransFilter
    {
        #region Status
        [PXDBString(1, IsUnicode = true)]
        [PXUIField(DisplayName = "Status")]
        [TWNStringList.TWNGUIManualStatus.List]
        [PXDefault(TWNStringList.TWNGUIManualStatus.Open)]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion
    }
    #endregion
}