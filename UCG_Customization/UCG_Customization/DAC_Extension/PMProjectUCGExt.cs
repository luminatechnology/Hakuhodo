using PX.Data;

namespace PX.Objects.PM
{
    public class PMProjectUCGExt : PXCacheExtension<PMProject>
    {
        #region Unbound
        #region IsApproving 
        [PXBool]
        [PXUIField(DisplayName = "IsApproving")]
        public virtual bool? IsApproving { get; set; }
        public abstract class isApproving : PX.Data.BQL.BqlBool.Field<isApproving> { }
        #endregion
        #endregion
    }
}
