using PX.Data;
using PX.TM;

namespace PX.Objects.EP
{
    public class EPDepartmentExt : PXCacheExtension<PX.Objects.EP.EPDepartment>
    {
        #region UsrWorkGroupID
        [PXDBInt]
        [PXUIField(DisplayName = "Work Group")]
        [PXSelector(
            typeof(EPCompanyTree.workGroupID),
            typeof(EPCompanyTree.description),
            SubstituteKey = typeof(EPCompanyTree.description),
            DescriptionField = typeof(EPCompanyTree.description)
            )]
        //[PXDBDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? UsrWorkGroupID { get; set; }
        public abstract class usrWorkGroupID : PX.Data.BQL.BqlInt.Field<usrWorkGroupID> { }
        #endregion
    }
}