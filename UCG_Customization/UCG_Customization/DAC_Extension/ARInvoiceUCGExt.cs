using PX.Data;
namespace PX.Objects.AR
{
    public class ARInvoiceUCGExt : PXCacheExtension<ARInvoice>
    {

        #region UsrCheckTaxOfTotal
        [PXDBBool]
        [PXUIField(DisplayName = "AR Tax equals to 5% of Detail Total")]
        [PXDefault(typeof(ARSetupUCGExt.usrCheckTaxOfTotal), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrCheckTaxOfTotal { get; set; }
        public abstract class usrCheckTaxOfTotal : PX.Data.BQL.BqlBool.Field<usrCheckTaxOfTotal> { }
        #endregion
    }
}
