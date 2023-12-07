using PX.Common;
using PX.Data;

namespace PX.Objects.GL
{
    public class JournalEntry_Extensions : PXGraphExtension<JournalEntry>
    {
        #region Event Handlers
        protected void _(Events.RowUpdating<GLTran> e, PXRowUpdating baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            // Since the special UOM value only applies to custom settlement ledgers, it cannot be automatically carried over to new journal transactions.
            if (Base.IsCopyPasteContext == true && e.Row?.UOM.IsIn(UCGLedgerSettlement.Graph.LSLedgerStlmtEntry.ZZ_UOM, UCGLedgerSettlement.Graph.LSLedgerStlmtEntry.YY_UOM) == true)
            {
                e.NewRow.UOM = null;
            }
        }
        #endregion
    }
}
