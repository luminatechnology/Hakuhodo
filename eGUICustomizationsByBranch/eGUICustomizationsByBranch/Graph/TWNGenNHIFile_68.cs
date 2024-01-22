using PX.Data;

namespace eGUICustomizationsByBranch.Graph
{
    public class TWNGenNHIFile_68 : TWNGenerateNHIFileBase
    {
        protected virtual void _(Events.FieldDefaulting<TWNGenerateNHIFileFilter.secNHICode> e)
        {   
            e.NewValue = "51";
        }

        protected virtual void _(Events.FieldDefaulting<TWNGenerateNHIFileFilter.secNHIType> e)
        {
            e.NewValue = "68";
        }
    }
}