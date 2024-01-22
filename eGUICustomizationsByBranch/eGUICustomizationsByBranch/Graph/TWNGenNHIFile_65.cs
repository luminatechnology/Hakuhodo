using PX.Data;

namespace eGUICustomizationsByBranch.Graph
{
    public class TWNGenNHIFile_65 : TWNGenerateNHIFileBase
    {
        protected virtual void _(Events.FieldDefaulting<TWNGenerateNHIFileFilter.secNHICode> e)
        {   
            e.NewValue = "9A,9B";
        }

        protected virtual void _(Events.FieldDefaulting<TWNGenerateNHIFileFilter.secNHIType> e)
        {
            e.NewValue = "65";
        }
    }
}