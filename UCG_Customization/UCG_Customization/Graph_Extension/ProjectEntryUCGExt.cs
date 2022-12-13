﻿using PX.Data;
using UCG_Customization.DAC;

namespace PX.Objects.PM
{
    public class ProjectEntryUCGExt:PXGraphExtension<ProjectEntry>
    {
        public static bool IsActive() {
            return true;
        }

        [PXViewName("PM Summary By BIV")]
        public PXSelect<PMSummaryByBIV,Where<PMSummaryByBIV.contractID,Equal<Current<PMProject.contractID>>>> BISummary;
    }

}