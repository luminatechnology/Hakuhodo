using PX.Data;
using PX.Objects.EP;
using Standalone = PX.Objects.CR.Standalone;

namespace PX.Objects.CR
{
    public class CROpportunityUCGExt : PXCacheExtension<CROpportunity>
    {
        #region UsrApprover1 
        [PXDBInt(BqlField =typeof(Standalone.CROpportunityUCGExt.usrApprover1))]
        [PXUIField(DisplayName = "UsrApprover1")]
        [PXSelector(typeof(Search<EPEmployee.bAccountID>),
            typeof(EPEmployee.acctCD),
            typeof(EPEmployee.acctName),
            typeof(EPEmployee.departmentID),
            SubstituteKey = typeof(EPEmployee.acctCD),
            DescriptionField = typeof(EPEmployee.acctName)
            )]
        public virtual int? UsrApprover1 { get; set; }
        public abstract class usrApprover1 : PX.Data.BQL.BqlInt.Field<usrApprover1> { }
        #endregion

        #region UsrApprover2 
        [PXDBInt(BqlField = typeof(Standalone.CROpportunityUCGExt.usrApprover2))]
        [PXUIField(DisplayName = "UsrApprover2")]
        [PXSelector(typeof(Search<EPEmployee.bAccountID>),
            typeof(EPEmployee.acctCD),
            typeof(EPEmployee.acctName),
            typeof(EPEmployee.departmentID),
            SubstituteKey = typeof(EPEmployee.acctCD),
            DescriptionField = typeof(EPEmployee.acctName)
            )]
        public virtual int? UsrApprover2 { get; set; }
        public abstract class usrApprover2 : PX.Data.BQL.BqlInt.Field<usrApprover2> { }
        #endregion

        #region UsrApprover3 
        [PXDBInt(BqlField = typeof(Standalone.CROpportunityUCGExt.usrApprover3))]
        [PXUIField(DisplayName = "UsrApprover3")]
        [PXSelector(typeof(Search<EPEmployee.bAccountID>),
            typeof(EPEmployee.acctCD),
            typeof(EPEmployee.acctName),
            typeof(EPEmployee.departmentID),
            SubstituteKey = typeof(EPEmployee.acctCD),
            DescriptionField = typeof(EPEmployee.acctName)
            )]
        public virtual int? UsrApprover3 { get; set; }
        public abstract class usrApprover3 : PX.Data.BQL.BqlInt.Field<usrApprover3> { }
        #endregion

        #region UsrApprover4 
        [PXDBInt(BqlField = typeof(Standalone.CROpportunityUCGExt.usrApprover4))]
        [PXUIField(DisplayName = "UsrApprover4")]
        [PXSelector(typeof(Search<EPEmployee.bAccountID>),
            typeof(EPEmployee.acctCD),
            typeof(EPEmployee.acctName),
            typeof(EPEmployee.departmentID),
            SubstituteKey = typeof(EPEmployee.acctCD),
            DescriptionField = typeof(EPEmployee.acctName)
            )]
        public virtual int? UsrApprover4 { get; set; }
        public abstract class usrApprover4 : PX.Data.BQL.BqlInt.Field<usrApprover4> { }
        #endregion

        #region UsrApprover5 
        [PXDBInt(BqlField = typeof(Standalone.CROpportunityUCGExt.usrApprover5))]
        [PXUIField(DisplayName = "UsrApprover5",Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXSelector(typeof(Search<EPEmployee.bAccountID>),
            typeof(EPEmployee.acctCD),
            typeof(EPEmployee.acctName),
            typeof(EPEmployee.departmentID),
            SubstituteKey = typeof(EPEmployee.acctCD),
            DescriptionField = typeof(EPEmployee.acctName)
            )]
        public virtual int? UsrApprover5 { get; set; }
        public abstract class usrApprover5 : PX.Data.BQL.BqlInt.Field<usrApprover5> { }
        #endregion
    }
}
