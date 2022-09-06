using PX.Data;

namespace PX.Objects.AP
{
    public class APRegisterWorkGroupExt : PXCacheExtension<PX.Objects.AP.APRegister>
    {

        #region UsrDepartmentID 
        [PXDBString]
        [PXUIField(DisplayName = "DepartmentID",IsReadOnly =true)]
        [PXDBDefault(PersistingCheck =PXPersistingCheck.Nothing)]
        public virtual string UsrDepartmentID { get; set; }
        public abstract class usrDepartmentID : PX.Data.BQL.BqlString.Field<usrDepartmentID> { }
        #endregion

        #region UsrApproveWG01 
        [PXDBInt]
        [PXUIField(DisplayName = "ApproveWG01")]

        public virtual int? UsrApproveWG01 { get; set; }
        public abstract class usrApproveWG01 : PX.Data.BQL.BqlInt.Field<usrApproveWG01> { }
        #endregion

        #region UsrApproveWG02 
        [PXDBInt]
        [PXUIField(DisplayName = "ApproveWG02")]
        public virtual int? UsrApproveWG02 { get; set; }
        public abstract class usrApproveWG02 : PX.Data.BQL.BqlInt.Field<usrApproveWG02> { }
        #endregion

        #region UsrApproveWG03 
        [PXDBInt]
        [PXUIField(DisplayName = "ApproveWG03")]
        public virtual int? UsrApproveWG03 { get; set; }
        public abstract class usrApproveWG03 : PX.Data.BQL.BqlInt.Field<usrApproveWG03> { }
        #endregion

        #region UsrApproveWG04 
        [PXDBInt]
        [PXUIField(DisplayName = "ApproveWG04")]
        public virtual int? UsrApproveWG04 { get; set; }
        public abstract class usrApproveWG04 : PX.Data.BQL.BqlInt.Field<usrApproveWG04> { }
        #endregion

        #region UsrApproveWG05 
        [PXDBInt]
        [PXUIField(DisplayName = "ApproveWG05")]
        public virtual int? UsrApproveWG05 { get; set; }
        public abstract class usrApproveWG05 : PX.Data.BQL.BqlInt.Field<usrApproveWG05> { }
        #endregion

        #region UsrApproveWG06 
        [PXDBInt]
        [PXUIField(DisplayName = "ApproveWG06")]
        public virtual int? UsrApproveWG06 { get; set; }
        public abstract class usrApproveWG06 : PX.Data.BQL.BqlInt.Field<usrApproveWG06> { }
        #endregion

        #region UsrApproveWG07 
        [PXDBInt]
        [PXUIField(DisplayName = "ApproveWG07")]
        public virtual int? UsrApproveWG07 { get; set; }
        public abstract class usrApproveWG07 : PX.Data.BQL.BqlInt.Field<usrApproveWG07> { }
        #endregion

        #region UsrApproveWG08 
        [PXDBInt]
        [PXUIField(DisplayName = "ApproveWG08")]
        public virtual int? UsrApproveWG08 { get; set; }
        public abstract class usrApproveWG08 : PX.Data.BQL.BqlInt.Field<usrApproveWG08> { }
        #endregion

        #region UsrApproveWG09 
        [PXDBInt]
        [PXUIField(DisplayName = "ApproveWG09")]
        public virtual int? UsrApproveWG09 { get; set; }
        public abstract class usrApproveWG09 : PX.Data.BQL.BqlInt.Field<usrApproveWG09> { }
        #endregion

        #region UsrApproveWG10 
        [PXDBInt]
        [PXUIField(DisplayName = "ApproveWG10")]
        public virtual int? UsrApproveWG10 { get; set; }
        public abstract class usrApproveWG10 : PX.Data.BQL.BqlInt.Field<usrApproveWG10> { }
        #endregion
    }
}