using System;
using System.Collections;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;
using PX.Objects.CS;
using PX.Objects.AR;

namespace eGUICustomizations.Graph
{
    public class TWNPrintedLineDetMaint : PXGraph<TWNPrintedLineDetMaint>
    {
        #region Features & Selets
        public PXSave<TWNPrintedLineFilter> Save;
        public PXCancel<TWNPrintedLineFilter> Cancel;
        public PXFilter<TWNPrintedLineFilter> Filter;
        [PXImport(typeof(TWNGUIPrintedLineDet))]
        public PXSelect<TWNGUIPrintedLineDet, Where<TWNGUIPrintedLineDet.gUINbr, Equal<Current<TWNPrintedLineFilter.gUINbr>>,
                                                    And<TWNGUIPrintedLineDet.gUIFormatcode, Equal<Current<TWNPrintedLineFilter.gUIFormatcode>>,
                                                        And<TWNGUIPrintedLineDet.refNbr, Equal<Current<TWNPrintedLineFilter.refNbr>>>>>> PrintedLineDet;
        #endregion

        #region Delegate Data View
        protected virtual IEnumerable filter()
        {
            TWNPrintedLineFilter filter = Filter.Current;

            if (filter != null)
            {
                filter.RevisedAmount = 0; //Reset total to zero 

                int startRow = 0, totalRows = 0;

                foreach (TWNGUIPrintedLineDet row in /*new PXView(this, false, cmd)*/PrintedLineDet.View.Select(new[] { filter }, //Pass filter context to the grid view delegate
                                                                                null, null, null, null,
                                                                                PrintedLineDet.View.GetExternalFilters(), //Get grid user filters
                                                                                ref startRow,
                                                                                0, //Get all records without regard to paging
                                                                                ref totalRows))
                {
                    filter.RevisedAmount += row.Amount;
                }

                if (filter.OrigAmount == null && filter?.GUINbr != null)
                {
                    TWNGUITrans gUITran = SelectFrom<TWNGUITrans>.Where<TWNGUITrans.gUINbr.IsEqual<@P.AsString>
                                                                        .And<TWNGUITrans.gUIFormatCode.IsEqual<@P.AsString>
                                                                             .And<TWNGUITrans.orderNbr.IsEqual<@P.AsString>>>>.View
                                                                 .SelectSingleBound(this, null, filter.GUINbr, filter.GUIFormatcode, filter.RefNbr);

                    filter.OrigAmount = string.IsNullOrEmpty(gUITran.TaxNbr) ? gUITran.NetAmount + gUITran.TaxAmount : gUITran.NetAmount;
                }
            }

            yield return filter;
        }
        #endregion

        #region Event Handlers
        protected virtual void _(Events.RowPersisting<TWNPrintedLineFilter> e)
        {
            if (e.Row.RevisedAmount > e.Row.OrigAmount) 
            {
                string errorMsg = "Revised Amount Cannot Be Greater Than Orig. Inv. Amt.";

                throw new PXSetPropertyException<TWNPrintedLineFilter.revisedAmount>(errorMsg);
            }
        }
        #endregion
    }

    [Serializable]
    [PXCacheName("TWN Printed Line Filter")]
    public class TWNPrintedLineFilter : IBqlTable
    {
        #region GUIFormatcode
        [PXDBString(2, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "GUI Format code")]
        [PXSelector(typeof(Search<CSAttributeDetail.valueID, Where<CSAttributeDetail.attributeID, Equal<ARRegisterExt.VATOUTFRMTNameAtt>,
                                                                   Or<CSAttributeDetail.attributeID, Equal<TWNManualGUIAPBill.VATINFRMTNameAtt>>>>),
                    typeof(CSAttributeDetail.valueID),
                    typeof(CSAttributeDetail.description),
                    DescriptionField = typeof(CSAttributeDetail.description))]
        public virtual string GUIFormatcode { get; set; }
        public abstract class gUIFormatcode : PX.Data.BQL.BqlString.Field<gUIFormatcode> { }
        #endregion

        #region RefNbr
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Ref. Number")]
        [PXSelector(typeof(SelectFrom<TWNGUITrans>.Where<TWNGUITrans.gUIFormatCode.IsEqual<gUIFormatcode.AsOptional>
                                                         .And<TWNGUITrans.gUIStatus.IsEqual<TWNStringList.TWNGUIStatus.used>>>.SearchFor<TWNGUITrans.orderNbr>),
                    typeof(TWNGUITrans.orderNbr),
                    typeof(TWNGUITrans.custVend),
                    typeof(TWNGUITrans.custVendName),
                    typeof(TWNGUITrans.transDate))]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region GUINbr
        [GUINumber(15, IsUnicode = true, InputMask = ">aaaaaaaaaaaaa")]
        [PXUIField(DisplayName = "GUI Number")]
        [PXSelector(typeof(SelectFrom<TWNGUITrans>.Where<TWNGUITrans.gUIStatus.IsEqual<TWNStringList.TWNGUIStatus.used>
                                                         .And<TWNGUITrans.eGUIExcluded.IsEqual<False>
                                                             .And<TWNGUITrans.orderNbr.IsEqual<refNbr.AsOptional>>>>.SearchFor<TWNGUITrans.gUINbr>),
                    typeof(TWNGUITrans.gUINbr),
                    typeof(TWNGUITrans.gUIDate),
                    typeof(TWNGUITrans.custVend),
                    typeof(TWNGUITrans.branchID),
                    typeof(TWNGUITrans.netAmount))]
        public virtual string GUINbr { get; set; }
        public abstract class gUINbr : PX.Data.BQL.BqlString.Field<gUINbr> { }
        #endregion

        #region OrigAmount
        [TWNetAmount(0)]
        [PXUIField(DisplayName = "Orig. Inv. Amt", Enabled = false)]
        public virtual decimal? OrigAmount { get; set; }
        public abstract class origAmount : PX.Data.BQL.BqlDecimal.Field<origAmount> { }
        #endregion

        #region RevisedAmount
        [TWNetAmount(0)]
        [PXUIField(DisplayName = "Revised Amount", Enabled = false)]
        public virtual decimal? RevisedAmount { get; set; }
        public abstract class revisedAmount : PX.Data.BQL.BqlDecimal.Field<revisedAmount> { }
        #endregion
    }
}