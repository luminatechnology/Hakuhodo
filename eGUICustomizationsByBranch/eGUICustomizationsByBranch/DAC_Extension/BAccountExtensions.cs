using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.GL;
using eGUICustomizations.Descriptor;

namespace PX.Objects.CR
{
    public class BAccountExt : PXCacheExtension<PX.Objects.CR.BAccount>
    {
        #region UsrTaxRegistrationID
        [PXDBString(9, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Registration")]
        public virtual string UsrTaxRegistrationID { get; set; }
        public abstract class usrTaxRegistrationID : PX.Data.BQL.BqlString.Field<usrTaxRegistrationID> { }
        #endregion
    
        #region UsrOurTaxNbr
        [TaxNbrVerify(8, IsUnicode = true)]
        [PXUIField(DisplayName = "Our Tax Nbr.")]
        public virtual string UsrOurTaxNbr { get; set; }
        public abstract class usrOurTaxNbr : PX.Data.BQL.BqlString.Field<usrOurTaxNbr> { }
        #endregion
    
        #region UsrZeroTaxTaxCntry
        [PXDBString(1, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Zero Tax Country")]
        public virtual string UsrZeroTaxTaxCntry { get; set; }
        public abstract class usrZeroTaxTaxCntry : PX.Data.BQL.BqlString.Field<usrZeroTaxTaxCntry> { }
        #endregion
    
        #region UsrCompanyName
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Company Name")]
        public virtual string UsrCompanyName { get; set; }
        public abstract class usrCompanyName : PX.Data.BQL.BqlString.Field<usrCompanyName> { }
        #endregion
    
        #region UsrAddressLine
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Address Line")]
        public virtual string UsrAddressLine { get; set; }
        public abstract class usrAddressLine : PX.Data.BQL.BqlString.Field<usrAddressLine> { }
        #endregion
    
        #region UsrAESKey
        [PXDBString(32, IsUnicode = true)]
        [PXUIField(DisplayName = "AES Key")]
        public virtual string UsrAESKey { get; set; }
        public abstract class usrAESKey : PX.Data.BQL.BqlString.Field<usrAESKey> { }
        #endregion
    
        #region UsrWHTTaxAuthority
        [PXDBString(3, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Withholding Tax Authority")]
        public virtual string UsrWHTTaxAuthority { get; set; }
        public abstract class usrWHTTaxAuthority : PX.Data.BQL.BqlString.Field<usrWHTTaxAuthority> { }
        #endregion

        #region Static Methods
        public static string GetOurTaxNbByBranch(PXCache cache, int? branchID)
        {
            return SelectFrom<BAccount2>.InnerJoin<Branch>.On<Branch.bAccountID.IsEqual<BAccount2.bAccountID>>
                                        .Where<Branch.branchID.IsEqual<@P.AsInt>>.View.Select(cache.Graph, branchID)
                                        .TopFirst?.GetExtension<BAccountExt>().UsrOurTaxNbr;
        }

        public static BAccountExt GetTWGUIByBranch(PXCache cache, int? branchID)
        {
            return SelectFrom<BAccount2>.InnerJoin<Branch>.On<Branch.bAccountID.IsEqual<BAccount2.bAccountID>>
                                        .Where<Branch.branchID.IsEqual<@P.AsInt>>.View.Select(cache.Graph, branchID)
                                        .TopFirst?.GetExtension<BAccountExt>();
        }
        #endregion
    }
}