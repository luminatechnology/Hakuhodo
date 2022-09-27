using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.GL;
using eGUICustomizations.DAC;

namespace eGUICustomizations.Graph
{
    public class TWNWHTInquiry : PXGraph<TWNWHTInquiry>
    {
        #region Select & Features
        public PXSavePerRow<TWNWHTTran> Save;
        public PXCancel<TWNWHTTran> Cancel;

        [PXFilterable]
        public SelectFrom<TWNWHTTran>.View WHTTran;
        #endregion

        #region Cache Attached
        [PXCustomizeBaseAttribute(typeof(PXDBStringAttribute), nameof(PXDBStringAttribute.IsKey), false)]
        protected virtual void _(Events.CacheAttached<TWNWHTTran.docType> e) { }

        [PXCustomizeBaseAttribute(typeof(PXDBStringAttribute), nameof(PXDBStringAttribute.IsKey), false)]
        protected virtual void _(Events.CacheAttached<TWNWHTTran.refNbr> e) { }

        [PXCustomizeBaseAttribute(typeof(PXDBStringAttribute), nameof(PXDBStringAttribute.IsKey), true)]
        protected virtual void _(Events.CacheAttached<TWNWHTTran.batchNbr> e) { }
        #endregion

        #region Event Handlers
        protected virtual void _(Events.RowSelected<TWNWHTTran> e)
        {
            PXUIFieldAttribute.SetEnabled<TWNWHTTran.docType>(e.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<TWNWHTTran.refNbr>(e.Cache, null, false);

            PXUIFieldAttribute.SetRequired<TWNWHTTran.batchNbr>(e.Cache, true);
            PXUIFieldAttribute.SetRequired<TWNWHTTran.branchID>(e.Cache, true);
            PXUIFieldAttribute.SetRequired<TWNWHTTran.tranDate>(e.Cache, true);
            PXUIFieldAttribute.SetRequired<TWNWHTTran.paymDate>(e.Cache, true);
        }

        protected virtual void _(Events.FieldUpdated<TWNWHTTran.batchNbr> e)
        {
            var row = e.Row as TWNWHTTran;

            row.DocType = row.RefNbr = string.Empty;

            Batch batch = SelectFrom<Batch>.Where<Batch.batchNbr.IsEqual<@P.AsString>>.View.SelectSingleBound(this, null, e.NewValue);

            row.BranchID = batch.BranchID;
            row.TranDate = batch.DateEntered;
        }
        #endregion
    }
}