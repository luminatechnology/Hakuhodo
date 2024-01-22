using PX.Data;

namespace eGUICustomizationsByBranch.Graph
{
    public class TWNGenNHIFile_63 : TWNGenerateNHIFileBase
    {
        protected virtual void _(Events.FieldDefaulting<TWNGenerateNHIFileFilter.secNHICode> e)
        {   
            e.NewValue = "50";
        }

        protected virtual void _(Events.FieldDefaulting<TWNGenerateNHIFileFilter.secNHIType> e)
        {
            e.NewValue = "63";
        }
    }
}