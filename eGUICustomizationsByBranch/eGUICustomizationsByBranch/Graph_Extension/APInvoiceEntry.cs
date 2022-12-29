using System.Linq;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.CR;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;

namespace PX.Objects.AP
{
    public class APInvoiceEntry_Extension : PXGraphExtension<APInvoiceEntry>
    {
        public bool activateGUI = TWNGUIValidation.ActivateTWGUI(new PXGraph());

        #region Selects        
        [PXCopyPasteHiddenFields(typeof(TWNManualGUIAPBill.docType),
                                 typeof(TWNManualGUIAPBill.refNbr),
                                 typeof(TWNManualGUIAPBill.gUINbr))]
        public SelectFrom<TWNManualGUIAPBill>.Where<TWNManualGUIAPBill.docType.IsEqual<APInvoice.docType.FromCurrent>
                                                    .And<TWNManualGUIAPBill.refNbr.IsEqual<APInvoice.refNbr.FromCurrent>>>
                                             .OrderBy<TWNManualGUIAPBill.createdDateTime.Asc>.View ManualAPBill;

        [PXCopyPasteHiddenFields(typeof(TWNWHT.docType), typeof(TWNWHT.refNbr))]
        public SelectFrom<TWNWHT>.Where<TWNWHT.docType.IsEqual<APInvoice.docType.FromCurrent>
                                        .And<TWNWHT.refNbr.IsEqual<APInvoice.refNbr.FromCurrent>>>.View WHTView;

        public SelectFrom<TWNGUIPreferences>.View GUISetup;
        #endregion

        #region Delegate Methods
        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            var invoice = Base.CurrentDocument.Current;

            if (invoice != null && string.IsNullOrEmpty(invoice.InvoiceNbr))
            {
                Base.CurrentDocument.Cache.SetValue<APInvoice.invoiceNbr>(invoice, ManualAPBill.Select().TopFirst?.GUINbr);
                Base.CurrentDocument.UpdateCurrent();
            }

            baseMethod();
        }
        #endregion

        #region Event Handlers
        protected void _(Events.RowPersisting<APInvoice> e, PXRowPersisting baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            var row = e.Row as APInvoice;

            if (row != null && row.DocType.IsIn(APDocType.Invoice, APDocType.DebitAdj) && string.IsNullOrEmpty(row.OrigRefNbr))
            {
                if (ManualAPBill.Select().Count == 0 && Base.Taxes.Select().Count > 0)
                {
                    foreach (TX.TaxTran tran in Base.Taxes.Cache.Cached)
                    {
                        if (TX.Tax.PK.Find(Base, tran.TaxID)?.GetExtension<TX.TaxExt>().UsrTWNGUI == true)
                        {
                            throw new PXException(TWMessages.NoGUIWithTax);
                        }
                    }
                }
                else
                {
                    if (e.Operation != PXDBOperation.Delete && e.Row.Hold == false)
                    {
                        if (row.TaxTotal != ManualAPBill.Select().RowCast<TWNManualGUIAPBill>().ToList().Sum(s => s.TaxAmt).Value)
                        {
                            var current = ManualAPBill.Current ?? ManualAPBill.Cache.Cached.RowCast<TWNManualGUIAPBill>().FirstOrDefault();

                            ManualAPBill.Cache.RaiseExceptionHandling<TWNManualGUIAPBill.taxAmt>(current, current?.TaxAmt, new PXSetPropertyException(TWMessages.ChkTotalGUIAmt));

                            throw new PXException(PX.Objects.Common.Messages.RecordCanNotBeSaved);
                        }
                    }
                }
            }
        }

        protected void _(Events.RowSelected<APInvoice> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            var row = e.Row as APInvoice;

            if (row != null)
            {
                ManualAPBill.Cache.AllowSelect = activateGUI;
                ManualAPBill.Cache.AllowDelete = ManualAPBill.Cache.AllowInsert = ManualAPBill.Cache.AllowUpdate = row.Status.IsIn(APDocStatus.Hold, APDocStatus.Balanced);
            }

            WHTView.AllowSelect = GUISetup.Select().TopFirst?.EnableWHT == true;
            WHTView.AllowDelete = WHTView.AllowInsert = WHTView.AllowUpdate = Base.Transactions.AllowUpdate;
        }

        protected void _(Events.FieldUpdated<APInvoice.taxZoneID> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            InsertDefaultWHT();
        }

        #region TWNManualGUIAPBill
        protected virtual void _(Events.RowInserting<TWNManualGUIAPBill> e)
        {
            e.Row.SortOrder = e.Cache.Cached.RowCast<TWNManualGUIAPBill>().Count() + 1;
        }

        protected virtual void _(Events.FieldDefaulting<TWNManualGUIAPBill, TWNManualGUIAPBill.deduction> e)
        {
            /// If user doesn't choose a vendor then bring the fixed default value from Attribure "DEDUCTCODE" first record.
            e.NewValue = e.Row.VendorID == null ? new string1() : e.NewValue;
        }

        protected virtual void _(Events.FieldDefaulting<TWNManualGUIAPBill, TWNManualGUIAPBill.branchID> e)
        {
            e.NewValue = Base.Document.Current?.BranchID;

            // Since the BranchAttribute will bring default value, it cannot immediately respond to the new value to the event and trigger the related event.
            ManualAPBill.Cache.SetValueExt<TWNManualGUIAPBill.ourTaxNbr>(e.Row, BAccountExt.GetOurTaxNbByBranch(e.Cache, (int?)e.NewValue));
        }

        protected virtual void _(Events.FieldDefaulting<TWNManualGUIAPBill, TWNManualGUIAPBill.vATInCode> e)
        {
            e.NewValue = Base.Document.Current?.DocType == APDocType.DebitAdj ? TWGUIFormatCode.vATInCode23 : e.NewValue;
        }

        protected virtual void _(Events.FieldDefaulting<TWNManualGUIAPBill, TWNManualGUIAPBill.createdDateTime> e)
        {
            e.NewValue = System.DateTime.UtcNow;
        }
        #endregion

        #endregion

        #region Methods
        /// <summary>
        /// TX206000 新增 “代扣稅相關” checkbox, AP301000 如果選出來的 供應商稅務區域 是與代扣稅相關，則將代扣稅相關欄位由供應商帶出.
        /// </summary>
        protected virtual void InsertDefaultWHT()
        {
            var invoice = Base.Document.Current;
            var vendor  = Base.vendor.Current;

            if (invoice == null || vendor == null || activateGUI == false) { return; }

            if (TX.TaxZone.PK.Find(Base, invoice.TaxZoneID)?.GetExtension<TX.TaxZoneExt>().UsrWHTTaxRelated == true)
            {
                TWNGUIPreferences pref = GUISetup.Select();

                if (pref?.EnableWHT == true && invoice.DocType == APDocType.Invoice)
                {
                    TWNWHT wNWHT = new TWNWHT()
                    {
                        DocType = invoice.DocType,
                        RefNbr = invoice.RefNbr
                    };

                    wNWHT = WHTView.Insert(wNWHT);

                    // Avoid returning an Null cache with the same PK.
                    if (wNWHT == null) { return; }

                    foreach (CSAnswers answers in SelectFrom<CSAnswers>.Where<CSAnswers.refNoteID.IsEqual<@P.AsGuid>>.View.Select(Base, vendor.NoteID))
                    {
                        switch (answers.AttributeID)
                        {
                            case TWNWHT.PersonalName:
                                wNWHT.PersonalID = answers.Value;
                                break;
                            case TWNWHT.PropertyName:
                                wNWHT.PropertyID = answers.Value;
                                break;
                            case TWNWHT.TypeOfInName:
                                wNWHT.TypeOfIn = answers.Value;
                                break;
                            case TWNWHT.WHTFmtCodeName:
                                wNWHT.WHTFmtCode = answers.Value;
                                break;
                            case TWNWHT.WHTFmtSubName:
                                wNWHT.WHTFmtSub = answers.Value;
                                break;
                            case TWNWHT.WHTTaxPctName:
                                wNWHT.WHTTaxPct = answers.Value;
                                break;
                            case TWNWHT.SecNHICodeName:
                                wNWHT.SecNHICode = answers.Value;
                                break;
                        }
                    }

                    wNWHT.SecNHIPct = pref?.SecGenerationNHIPct;

                    WHTView.Cache.Update(wNWHT);
                }
            }
        }
        #endregion
    }
}