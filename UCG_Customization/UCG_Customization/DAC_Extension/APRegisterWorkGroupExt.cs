using PX.Data;

namespace PX.Objects.AP
{
    public class APRegisterWorkGroupExt : PXCacheExtension<PX.Objects.AP.APRegister>
    {

        #region UsrDepartmentID 
        [PXDBString]
        [PXUIField(DisplayName = "DepartmentID", IsReadOnly = true)]
        [PXDBDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string UsrDepartmentID { get; set; }
        public abstract class usrDepartmentID : PX.Data.BQL.BqlString.Field<usrDepartmentID> { }
        #endregion

        #region UsrApproveWG01 
        [PXDBString]
        [PXUIField(DisplayName = "ApproveWG01")]

        public virtual string UsrApproveWG01 { get; set; }
        public abstract class usrApproveWG01 : PX.Data.BQL.BqlString.Field<usrApproveWG01> { }
        #endregion

        #region UsrApproveWG02 
        [PXDBString]
        [PXUIField(DisplayName = "ApproveWG02")]
        public virtual string UsrApproveWG02 { get; set; }
        public abstract class usrApproveWG02 : PX.Data.BQL.BqlString.Field<usrApproveWG02> { }
        #endregion

        #region UsrApproveWG03 
        [PXDBString]
        [PXUIField(DisplayName = "ApproveWG03")]
        public virtual string UsrApproveWG03 { get; set; }
        public abstract class usrApproveWG03 : PX.Data.BQL.BqlString.Field<usrApproveWG03> { }
        #endregion

        #region UsrApproveWG04 
        [PXDBString]
        [PXUIField(DisplayName = "ApproveWG04")]
        public virtual string UsrApproveWG04 { get; set; }
        public abstract class usrApproveWG04 : PX.Data.BQL.BqlString.Field<usrApproveWG04> { }
        #endregion

        #region UsrApproveWG05 
        [PXDBString]
        [PXUIField(DisplayName = "ApproveWG05")]
        public virtual string UsrApproveWG05 { get; set; }
        public abstract class usrApproveWG05 : PX.Data.BQL.BqlString.Field<usrApproveWG05> { }
        #endregion

        #region UsrApproveWG06 
        [PXDBString]
        [PXUIField(DisplayName = "ApproveWG06")]
        public virtual string UsrApproveWG06 { get; set; }
        public abstract class usrApproveWG06 : PX.Data.BQL.BqlString.Field<usrApproveWG06> { }
        #endregion

        #region UsrApproveWG07 
        [PXDBString]
        [PXUIField(DisplayName = "ApproveWG07")]
        public virtual string UsrApproveWG07 { get; set; }
        public abstract class usrApproveWG07 : PX.Data.BQL.BqlString.Field<usrApproveWG07> { }
        #endregion

        #region UsrApproveWG08 
        [PXDBString]
        [PXUIField(DisplayName = "ApproveWG08")]
        public virtual string UsrApproveWG08 { get; set; }
        public abstract class usrApproveWG08 : PX.Data.BQL.BqlString.Field<usrApproveWG08> { }
        #endregion

        #region UsrApproveWG09 
        [PXDBString]
        [PXUIField(DisplayName = "ApproveWG09")]
        public virtual string UsrApproveWG09 { get; set; }
        public abstract class usrApproveWG09 : PX.Data.BQL.BqlString.Field<usrApproveWG09> { }
        #endregion

        #region UsrApproveWG10 
        [PXDBString]
        [PXUIField(DisplayName = "ApproveWG10")]
        public virtual string UsrApproveWG10 { get; set; }
        public abstract class usrApproveWG10 : PX.Data.BQL.BqlString.Field<usrApproveWG10> { }
        #endregion
    }
}