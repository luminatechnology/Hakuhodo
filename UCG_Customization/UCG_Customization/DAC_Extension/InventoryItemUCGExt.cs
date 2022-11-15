using PX.Data;

namespace PX.Objects.IN
{
    public class InventoryItemUCGExt : PXCacheExtension<InventoryItem>
    {
        #region UsrIsMiscExp
        [PXDBBool]
        [PXUIField(DisplayName = "Is MiscExp")]
        public virtual bool? UsrIsMiscExp { get; set; }
        public abstract class usrIsMiscExp : PX.Data.BQL.BqlBool.Field<usrIsMiscExp> { }
        #endregion
    }
}
