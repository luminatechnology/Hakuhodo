using System;
using PX.Data;

namespace UCG_Customization.DAC
{
    [Serializable]
    [PXCacheName("APARTeleTransView")]
    public class APARTeleTransView : IBqlTable
    {
        #region Selected
        [PXBool()]
        [PXUIField(DisplayName = "Selected")]
        [PXUnboundDefault(false)]
        public virtual bool? Selected { get; set; }
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        #endregion

        #region DataType
        [PXDBString(2, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Data Type")]
        public virtual string DataType { get; set; }
        public abstract class dataType : PX.Data.BQL.BqlString.Field<dataType> { }
        #endregion

        #region RefNbr
        [PXDBString(15, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Ref Nbr")]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region DocType
        [PXDBString(3, IsFixed = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Doc Type")]
        public virtual string DocType { get; set; }
        public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
        #endregion

        #region Branch
        [PXDBInt()]
        [PXUIField(DisplayName = "Branch")]
        public virtual int? Branch { get; set; }
        public abstract class branch : PX.Data.BQL.BqlInt.Field<branch> { }
        #endregion

        #region BatchNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Batch Nbr")]
        public virtual string BatchNbr { get; set; }
        public abstract class batchNbr : PX.Data.BQL.BqlString.Field<batchNbr> { }
        #endregion

        #region PaymentMethodID
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Method ID")]
        public virtual string PaymentMethodID { get; set; }
        public abstract class paymentMethodID : PX.Data.BQL.BqlString.Field<paymentMethodID> { }
        #endregion

        #region CashAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Cash Account ID")]
        public virtual int? CashAccountID { get; set; }
        public abstract class cashAccountID : PX.Data.BQL.BqlInt.Field<cashAccountID> { }
        #endregion

        #region PayBankBranch
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Pay Bank Branch")]
        public virtual string PayBankBranch { get; set; }
        public abstract class payBankBranch : PX.Data.BQL.BqlString.Field<payBankBranch> { }
        #endregion

        #region TaxRegistrationID
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tax Registration ID")]
        public virtual string TaxRegistrationID { get; set; }
        public abstract class taxRegistrationID : PX.Data.BQL.BqlString.Field<taxRegistrationID> { }
        #endregion

        #region DocDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Doc Date")]
        public virtual DateTime? DocDate { get; set; }
        public abstract class docDate : PX.Data.BQL.BqlDateTime.Field<docDate> { }
        #endregion

        #region DocDesc
        [PXDBString(512, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Doc Desc")]
        public virtual string DocDesc { get; set; }
        public abstract class docDesc : PX.Data.BQL.BqlString.Field<docDesc> { }
        #endregion

        #region BAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "BAccount ID")]
        public virtual int? BAccountID { get; set; }
        public abstract class bAccountID : PX.Data.BQL.BqlInt.Field<bAccountID> { }
        #endregion

        #region BAccountTWID
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "BAccount TWID")]
        public virtual string BAccountTWID { get; set; }
        public abstract class bAccountTWID : PX.Data.BQL.BqlString.Field<bAccountTWID> { }
        #endregion

        #region PostalCode
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Postal Code")]
        public virtual string PostalCode { get; set; }
        public abstract class postalCode : PX.Data.BQL.BqlString.Field<postalCode> { }
        #endregion

        #region EMail
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "EMail")]
        public virtual string EMail { get; set; }
        public abstract class eMail : PX.Data.BQL.BqlString.Field<eMail> { }
        #endregion

        #region AcctName
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Acct Name")]
        public virtual string AcctName { get; set; }
        public abstract class acctName : PX.Data.BQL.BqlString.Field<acctName> { }
        #endregion

        #region CuryOrigDocAmt
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Cury Orig Doc Amt")]
        public virtual Decimal? CuryOrigDocAmt { get; set; }
        public abstract class curyOrigDocAmt : PX.Data.BQL.BqlDecimal.Field<curyOrigDocAmt> { }
        #endregion

        #region Curyid
        [PXDBString(5, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Curyid")]
        public virtual string Curyid { get; set; }
        public abstract class curyid : PX.Data.BQL.BqlString.Field<curyid> { }
        #endregion

        #region IsTTGenerated
        [PXDBBool()]
        [PXUIField(DisplayName = "Is TTGenerated")]
        public virtual bool? IsTTGenerated { get; set; }
        public abstract class isTTGenerated : PX.Data.BQL.BqlBool.Field<isTTGenerated> { }
        #endregion

        #region PayAccount
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Pay Account")]
        public virtual string PayAccount { get; set; }
        public abstract class payAccount : PX.Data.BQL.BqlString.Field<payAccount> { }
        #endregion

        #region BankBranch
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank Branch")]
        public virtual string BankBranch { get; set; }
        public abstract class bankBranch : PX.Data.BQL.BqlString.Field<bankBranch> { }
        #endregion
    }
}