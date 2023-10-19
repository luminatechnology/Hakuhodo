using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO;

namespace PX.Objects.GL
{
    public class JournalEntry_Extensions : PXGraphExtension<JournalEntry>
    {
        #region Event Handlers
        protected void _(Events.RowUpdating<GLTran> e, PXRowUpdating baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            // Since the special UOM value only applies to custom settlement ledgers, it cannot be automatically carried over to new journal transactions.
            if (Base.IsCopyPasteContext == true && e.Row?.UOM.IsIn(HSNFinance.LSLedgerStlmtEntry.ZZ_UOM, HSNFinance.LSLedgerStlmtEntry.YY_UOM) == true)
            {
                e.NewRow.UOM = null;
            }
        }
        #endregion
    }
}
