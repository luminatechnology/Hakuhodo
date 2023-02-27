using System;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.AR;

namespace PX.Objects.PM
{
    public class PMBillEngineUCGExt : PXGraphExtension<PMBillEngine>
    {
        #region Override
        public delegate PMProformaLine InsertTransactionDelegate(PMProject project, PMProformaLine tran, string subCD, string note, Guid[] files);
        [PXOverride]
        public virtual PMProformaLine InsertTransaction(PMProject project, PMProformaLine tran, string subCD, string note, Guid[] files, InsertTransactionDelegate baseMethod)
        {
            var returnValue = baseMethod(project, tran, subCD, note, files);
            if (tran.Type != PMProformaLineType.Transaction)
            {
                var tax = TX.TaxCategory.PK.Find(Base, "TAXABLE");
                if (tax != null)
                    Base.ProformaEntry.Caches[typeof(PMProformaProgressLine)].SetValueExt<PMProformaProgressLine.taxCategoryID>(returnValue, tax.TaxCategoryID);
                Base.ProformaEntry.ProgressiveLines.Update((PMProformaProgressLine)returnValue);
            }

            return returnValue;
        }
        #endregion
    }
}
