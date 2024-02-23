using PX.Data;
using PX.Objects.PM;
using UCGLedgerSettlement.Descriptor;

namespace UCGLedgerSettlement.DAC_Extensions
{
    [PXNonInstantiatedExtension]
    public sealed class LSLedgerSettlement_ExistingColumn : PXCacheExtension<UCGLedgerSettlement.DAC.LSLedgerSettlement>
    {
        public static bool IsActive()
        {
            // Acuminator disable once PX1056 PXGraphCreationInIsActiveMethod [Justification]
            return LSSettlementFeaturesHelper.SettlementEdition; 
        }

        #region ProjectID
        [PXRemoveBaseAttribute(typeof(ActiveProjectOrContractForGLAttribute))]
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [Project()]
        public int? ProjectID { get; set; }
        #endregion

        #region TaskID
        [PXRemoveBaseAttribute(typeof(ActiveProjectTaskAttribute))]
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [ProjectTask(typeof(DAC.LSLedgerSettlement.projectID))]
        public int? TaskID { get; set; }
        #endregion
    }
}
