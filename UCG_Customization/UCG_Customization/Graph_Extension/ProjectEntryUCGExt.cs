using PX.Data;
using UCG_Customization.DAC;

namespace PX.Objects.PM
{
    public class ProjectEntryUCGExt:PXGraphExtension<ProjectEntry>
    {
        public PXSelectReadonly<PMSummaryByBIV,Where<PMSummaryByBIV.contractID,Equal<Current<PMProject.contractID>>>> BISummary;
    }
}
