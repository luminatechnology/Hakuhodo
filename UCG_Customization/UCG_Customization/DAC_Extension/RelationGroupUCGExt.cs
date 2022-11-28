using PX.Data;

namespace PX.SM
{
    public class RelationGroupUCGExt :PXCacheExtension<RelationGroup>
    {
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBString(255, IsUnicode = true)]
        protected virtual void _(Events.CacheAttached<RelationGroup.description> e) { }
    }
}
