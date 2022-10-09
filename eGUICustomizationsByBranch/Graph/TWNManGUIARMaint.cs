using System.Collections;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.TX;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;
using eGUICustomizations.Graph_Release;

namespace eGUICustomizations.Graph
{
    public class TWNManGUIAREntry : PXGraph<TWNManGUIAREntry>
    {
        public TWNReleaseProcess rp = PXGraph.CreateInstance<TWNReleaseProcess>();

        #region Selects & Setup
        public PXSave<TWNManualGUIAR> Save;
        public PXCancel<TWNManualGUIAR> Cancel;

        [PXImport(typeof(TWNManualGUIAR))]
        [PXFilterable]
        public SelectFrom<TWNManualGUIAR>
                          .Where<TWNManualGUIAR.status.IsEqual<TWNStringList.TWNGUIManualStatus.open>>.View manualGUIAR_Open;
        public SelectFrom<TWNManualGUIAR>
                          .Where<TWNManualGUIAR.status.IsEqual<TWNStringList.TWNGUIManualStatus.released>>.View.ReadOnly manualGUIAR_Released;

        public SelectFrom<TWNGUITrans>
                         .Where<TWNGUITrans.gUINbr.IsEqual<TWNManualGUIAR.gUINbr.FromCurrent>>.View ViewGUITrans;

        public PXSetup<TWNGUIPreferences> GUIPreferences;
        #endregion

        #region Overrie Methods
        public override void Persist()
        {
            foreach (TWNManualGUIAR row in manualGUIAR_Open.Cache.Deleted)
            {
                if (tWNGUIValidation.isCreditNote == false && !string.IsNullOrEmpty(row.GUINbr))
                {
                    InsertGUITran(row, Customer.PK.Find(this, row.CustomerID), false);

                    //Customer customer = Customer.PK.Find(this, row.CustomerID);
                    //rp.CreateGUITrans(new STWNGUITran()
                    //{
                    //    VATCode       = row.VatOutCode,
                    //    GUINbr        = row.GUINbr,
                    //    GUIStatus     = TWNStringList.TWNGUIStatus.Voided,
                    //    BranchID      = row.BranchID,
                    //    GUIDirection  = TWNStringList.TWNGUIDirection.Issue,
                    //    GUIDate       = row.GUIDate,
                    //    TaxZoneID     = row.TaxZoneID,
                    //    TaxCategoryID = row.TaxCategoryID,
                    //    TaxID         = row.TaxID,
                    //    TaxNbr        = row.TaxNbr,
                    //    OurTaxNbr     = row.OurTaxNbr,
                    //    NetAmount     = 0,
                    //    TaxAmount     = 0,
                    //    AcctCD        = customer.AcctCD,
                    //    AcctName      = customer.AcctName,
                    //    Remark        = string.Format(TWMessages.DeleteInfo, this.Accessinfo.UserName),
                    //    eGUIExcluded  = row.VatOutCode == TWGUIFormatCode.vATOutCode32,
                    //    OrderNbr      = row.GUINbr
                    //});
                }
            }

            base.Persist();
        }
        #endregion

        #region Actions
        public PXAction<TWNManualGUIAR> Release;
        [PXProcessButton(), PXUIField(DisplayName = ActionsMessages.Release)]
        public virtual IEnumerable release(PXAdapter adapter)
        {
            TWNManualGUIAR manualGUIAR = this.manualGUIAR_Open.Current;

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

                        //rp.CreateGUITrans(new STWNGUITran()
                        //{
                        //    VATCode       = manualGUIAR.VatOutCode,
                        //    GUINbr        = manualGUIAR.GUINbr,
                        //    GUIStatus     = TWNStringList.TWNGUIStatus.Used,
                        //    BranchID      = manualGUIAR.BranchID,
                        //    GUIDirection  = TWNStringList.TWNGUIDirection.Issue,
                        //    GUIDate       = manualGUIAR.GUIDate,
                        //    GUITitle      = customer.AcctName,
                        //    TaxZoneID     = manualGUIAR.TaxZoneID,
                        //    TaxCategoryID = manualGUIAR.TaxCategoryID,
                        //    TaxID         = manualGUIAR.TaxID,
                        //    TaxNbr        = manualGUIAR.TaxNbr,
                        //    OurTaxNbr     = manualGUIAR.OurTaxNbr,
                        //    NetAmount     = manualGUIAR.NetAmt,
                        //    TaxAmount     = manualGUIAR.TaxAmt,
                        //    AcctCD        = customer.AcctCD,
                        //    AcctName      = customer.AcctName,
                        //    eGUIExcluded  = manualGUIAR.VatOutCode == TWGUIFormatCode.vATOutCode32,
                        //    Remark        = manualGUIAR.Remark,
                        //    OrderNbr      = manualGUIAR.GUINbr
                        //});
                        InsertGUITran(manualGUIAR, Customer.PK.Find(this, manualGUIAR.CustomerID), false);

                        manualGUIAR.Status = TWNStringList.TWNGUIManualStatus.Released;
                        manualGUIAR_Open.Cache.MarkUpdated(manualGUIAR);

                        Actions.PressSave();

                        if (tWNGUITrans != null)
                        {
                            ViewGUITrans.SetValueExt<TWNGUITrans.netAmtRemain>(tWNGUITrans, (tWNGUITrans.NetAmtRemain -= manualGUIAR.NetAmt));
                            ViewGUITrans.SetValueExt<TWNGUITrans.taxAmtRemain>(tWNGUITrans, (tWNGUITrans.TaxAmtRemain -= manualGUIAR.TaxAmt));
                            ViewGUITrans.Update(tWNGUITrans);
                        }

                        ts.Complete();
                    }
                });
            }

            return adapter.Get();
        }
        #endregion

        #region Event Handlers
        TWNGUIValidation tWNGUIValidation = new TWNGUIValidation();

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

            row.OurTaxNbr = BAccountExt.GetOurTaxNbBymBranch(e.Cache, (int?)e.NewValue);
        }
        #endregion

        #region Methods
        private void InsertGUITran(TWNManualGUIAR data, Customer customer, bool isDeleted)
        {
            rp.CreateGUITrans(new STWNGUITran()
            {
                VATCode       = data.VatOutCode,
                GUINbr        = data.GUINbr,
                GUIStatus     = isDeleted == false ? TWNStringList.TWNGUIStatus.Used : TWNStringList.TWNGUIStatus.Voided,
                BranchID      = data.BranchID,
                GUIDirection  = TWNStringList.TWNGUIDirection.Issue,
                GUIDate       = data.GUIDate,
                GUITitle      = customer.AcctName,
                TaxZoneID     = data.TaxZoneID,
                TaxCategoryID = data.TaxCategoryID,
                TaxID         = data.TaxID,
                TaxNbr        = data.TaxNbr,
                OurTaxNbr     = data.OurTaxNbr,
                NetAmount     = isDeleted == false ? data.NetAmt : 0m,
                TaxAmount     = isDeleted == false ? data.TaxAmt : 0m,
                AcctCD        = customer.AcctCD,
                AcctName      = customer.AcctName,
                eGUIExcluded  = data.VatOutCode == TWGUIFormatCode.vATOutCode32,
                Remark        = isDeleted == false ? data.Remark : string.Format(TWMessages.DeleteInfo, this.Accessinfo.UserName),
                OrderNbr      = data.GUINbr
            });
        }
        #endregion
    }
}