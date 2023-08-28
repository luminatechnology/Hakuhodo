using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CT;

namespace PX.Objects.PO.GraphExtensions.APInvoiceEntryExt
{
    public class PreaymentsUCGExt : PXGraphExtension<Prepayments, APInvoiceEntry>
    {
        #region Override
        public delegate void AddPOOrderProcDelegate(POOrder order, bool createNew);

        [PXOverride]
        public virtual void AddPOOrderProc(POOrder order, bool createNew, AddPOOrderProcDelegate baseMethod)
        {
            baseMethod(order, createNew);
            var doc = Base.Document.Current;
            var contract = Contract.PK.Find(Base, order.ProjectID);
            Base.Document.Cache.SetValueExt(doc, APInvoiceEntry_WorkGroupExtension.UD_PROJECT, contract?.ContractCD);
        }
        #endregion
    }
}
