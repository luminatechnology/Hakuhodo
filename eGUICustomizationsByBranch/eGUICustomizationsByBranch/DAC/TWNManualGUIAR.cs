using System;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR.MassProcess;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.TX;
using eGUICustomizations.Descriptor;

namespace eGUICustomizations.DAC
{
    [Serializable]
    [PXCacheName("Manual GUI Trans (AR)")]
    public class TWNManualGUIAR : PX.Data.IBqlTable
    {
        #region RecordID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Record ID", Visible = false)]
        public virtual int? RecordID { get; set; }
        public abstract class recordID : PX.Data.BQL.BqlInt.Field<recordID> { }
        #endregion

        #region Status
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        [TWNStringList.TWNGUIManualStatus.List]
        [PXDefault(TWNStringList.TWNGUIManualStatus.Open)]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region CustomerID
        //[CustomerActive()]
        [PXDBInt()]
        [PXUIField(DisplayName = "Customer/Vendor")]
        [MultiBAccountSelector]
        public virtual int? CustomerID { get; set; }
        public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
        #endregion
    
        #region VatOutCode
        [PXDBString(2, IsUnicode = true)]
        [PXUIField(DisplayName = "VAT Out Code")]
        [PXSelector(typeof(Search<CSAttributeDetail.valueID,
                                  Where<CSAttributeDetail.attributeID, Equal<ARRegisterExt.VATOUTFRMTNameAtt>>>),
                    typeof(CSAttributeDetail.description))]
        [PXDefault(TWGUIFormatCode.vATOutCode32)]
        public virtual string VatOutCode { get; set; }
        public abstract class vatOutCode : PX.Data.BQL.BqlString.Field<vatOutCode> { }
        #endregion

        #region GUINbr
        [GUINumber(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "GUI Nbr.", Required = true)]
        [PXDefault()]
        //[AutoNumber(typeof(Search<TWNGUIPreferences.gUI2CopiesNumbering>), typeof(gUIDate))]
        [GUINbrAutoNumberAttribute.Numbering(typeof(vatOutCode), typeof(gUIDate),
                                             new string[] { TWGUIFormatCode.vATOutCode31, TWGUIFormatCode.vATOutCode32, TWGUIFormatCode.vATOutCode35 },
                                             new Type[] { typeof(TWNGUIPreferences.gUI3CopiesManNumbering),
                                                          typeof(TWNGUIPreferences.gUI2CopiesNumbering),
                                                          typeof(TWNGUIPreferences.gUI3CopiesNumbering) })]
        [PXSelector(typeof(Search<TWNGUITrans.gUINbr,
                                  Where<TWNGUITrans.gUIStatus, Equal<TWNStringList.TWNGUIStatus.used>,
                                        And<TWNGUITrans.gUIDirection, Equal<TWNStringList.TWNGUIDirection.issue>,
                                            And<TWNGUITrans.gUIFormatCode, NotEqual<ARRegisterExt.VATOut33Att>,
                                                And<TWNGUITrans.taxNbr, Equal<Current<taxNbr>>>>>>>),
                    typeof(TWNGUITrans.gUIFormatCode),
                    typeof(TWNGUITrans.netAmtRemain),
                    typeof(TWNGUITrans.taxAmtRemain),
                    Filterable = true,
                    ValidateValue = false)]
        public virtual string GUINbr { get; set; }
        public abstract class gUINbr : PX.Data.BQL.BqlString.Field<gUINbr> { }
        #endregion
    
        #region GUIDate
        [PXDBDate()]
        [PXUIField(DisplayName = "GUI Date")]
        [PXDefault()]
        public virtual DateTime? GUIDate { get; set; }
        public abstract class gUIDate : PX.Data.BQL.BqlDateTime.Field<gUIDate> { }
        #endregion
    
        #region TaxZoneID
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Zone", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search3<TaxZone.taxZoneID, OrderBy<Asc<TaxZone.taxZoneID>>>), CacheGlobal = true)]
        [PX.Data.EP.PXFieldDescription]
        public virtual string TaxZoneID { get; set; }
        public abstract class taxZoneID : PX.Data.BQL.BqlString.Field<taxZoneID> { }
        #endregion
    
        #region TaxCategoryID
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Category")]
        [PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
        [PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), PX.Objects.TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
        public virtual string TaxCategoryID { get; set; }
        public abstract class taxCategoryID : PX.Data.BQL.BqlString.Field<taxCategoryID> { }
        #endregion
   
        #region TaxID
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax ID", Visibility = PXUIVisibility.Visible)]
        [PXSelector(typeof(Tax.taxID), 
                    DescriptionField = typeof(Tax.descr), 
                    DirtyRead = true)]
        public virtual string TaxID { get; set; }
        public abstract class taxID : PX.Data.BQL.BqlString.Field<taxID> { }
        #endregion
    
        #region TaxNbr
        [TaxNbrVerify(8, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Nbr")]
        public virtual string TaxNbr { get; set; }
        public abstract class taxNbr : PX.Data.BQL.BqlString.Field<taxNbr> { }
        #endregion
    
        #region OurTaxNbr     
        [TaxNbrVerify(8, IsUnicode = true)]
        [PXUIField(DisplayName = "Our Tax Nbr.")]
        public virtual string OurTaxNbr { get; set; }
        public abstract class ourTaxNbr : PX.Data.BQL.BqlString.Field<ourTaxNbr> { }
        #endregion

        #region NetAmt
        [TWTaxAmountCalc(0, typeof(taxID), typeof(netAmt), typeof(taxAmt))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Net Amt")]
        public virtual decimal? NetAmt { get; set; }
        public abstract class netAmt : PX.Data.BQL.BqlDecimal.Field<netAmt> { }
        #endregion
    
        #region TaxAmt
        [TWTaxAmount(0, typeof(netAmt))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Tax Amt")]
        public virtual decimal? TaxAmt { get; set; }
        public abstract class taxAmt : PX.Data.BQL.BqlDecimal.Field<taxAmt> { }
        #endregion
    
        #region ExportMethod
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Export Method")]
        [ExportMthSelector]
        public virtual string ExportMethod { get; set; }
        public abstract class exportMethod : PX.Data.BQL.BqlString.Field<exportMethod> { }
        #endregion
    
        #region CustomType
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Custom Type")]
        [TWNStringList.TWNGUICustomType.List] 
        public virtual string CustomType { get; set; }
        public abstract class customType : PX.Data.BQL.BqlString.Field<customType> { }
        #endregion

        #region ExportTicketType
        [PXDBString(2, IsUnicode = true)]
        [PXUIField(DisplayName = "Export Ticket Type")]
        [ExportTickTypeSelector]
        public virtual string ExportTicketType { get; set; }
        public abstract class exportTicketType : PX.Data.BQL.BqlString.Field<exportTicketType> { }
        #endregion

        #region ExportTicketNbr
        [PXDBString(14, IsUnicode = true)]
        [PXUIField(DisplayName = "Export Ticket Nbr")]
        public virtual string ExportTicketNbr { get; set; }
        public abstract class exportTicketNbr : PX.Data.BQL.BqlString.Field<exportTicketNbr> { }
        #endregion
    
        #region ClearingDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Clearing Date")]
        public virtual DateTime? ClearingDate { get; set; }
        public abstract class clearingDate : PX.Data.BQL.BqlDateTime.Field<clearingDate> { }
        #endregion
        
        #region Remark
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion

        #region BranchID
        [Branch(useDefaulting: false)]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region GUITitle
        [PXDBString(80, IsUnicode = true)]
        [PXUIField(DisplayName = "GUI Title")]
        public virtual string GUITitle { get; set; }
        public abstract class gUITitle : PX.Data.BQL.BqlString.Field<gUITitle> { }
        #endregion

        #region AddressLine       
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "GUI Address")]
        [PXMassMergableField]
        [PXPersonalDataField]
        public virtual string AddressLine { get; set; }
        public abstract class addressLine : PX.Data.BQL.BqlString.Field<addressLine> { }
        #endregion

        #region B2CPrinted
        [PXDBBool()]
        [PXUIField(DisplayName = "B2C Printed")]
        [PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? B2CPrinted { get; set; }
        public abstract class b2CPrinted : PX.Data.BQL.BqlBool.Field<b2CPrinted> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime]
        [PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.CreatedDateTime,
                   Enabled = false,
                   IsReadOnly = true)]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID       
        [PXDBLastModifiedByScreenID]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime]
        [PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.LastModifiedDateTime,
                   Enabled = false,
                   IsReadOnly = true)]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region NoteID
        [PXNote]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #region tstamp        
        [PXDBTimestamp]
        public virtual byte[] tstamp { get; set; }
        public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }
        #endregion
    }
}