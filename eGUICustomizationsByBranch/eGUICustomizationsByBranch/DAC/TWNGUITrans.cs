using System;
using PX.Data;
using PX.Objects.CR.MassProcess;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.TX;
using eGUICustomizations.Graph;
using eGUICustomizations.Descriptor;
using Messages = PX.Objects.TX.Messages;
using static PX.Objects.AR.ARInvoiceEntry_Extension;
using SearchCategory = PX.Objects.SM.SearchCategory;

namespace eGUICustomizations.DAC
{
    [Serializable]
    [PXCacheName(TWMessages.GUICacheName)]
    [PXPrimaryGraph(typeof(TWNeGUIInquiry))]
    public class TWNGUITrans : PX.Data.IBqlTable
    {
        #region Selected       
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Selected", Visible = true, Visibility = PXUIVisibility.Invisible)]
        public bool? Selected { get; set; }
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        #endregion

        #region IdentityID
        [PXDBIdentity(IsKey = true)]
        public virtual int? IdentityID { get; set; }
        public abstract class identityID : PX.Data.BQL.BqlInt.Field<identityID> { }
        #endregion

        #region GUIFormatCode
        [PXDBString(2, IsUnicode = true)]
        [PXUIField(DisplayName = "GUI Format Code")]
        public virtual string GUIFormatCode { get; set; }
        public abstract class gUIFormatCode : PX.Data.BQL.BqlString.Field<gUIFormatCode> { }
        #endregion

        #region GUINbr
        [GUINumber(15, IsUnicode = true, InputMask = ">aaaaaaaaaaaaaaa")]
        [PXUIField(DisplayName = "GUI Nbr.")]
        [PXSelector(typeof(Search<TWNGUITrans.gUINbr, Where<TWNGUITrans.gUIStatus, Equal<TWNStringList.TWNGUIStatus.used>,
                                                            And<TWNGUITrans.gUIDirection, Equal<TWNStringList.TWNGUIDirection.issue>>>>),
                    typeof(TWNGUITrans.gUINbr),
                    typeof(TWNGUITrans.gUIFormatCode),
                    typeof(TWNGUITrans.branchID),
                    typeof(TWNGUITrans.gUIDate),
                    ValidateValue = false)]
        public virtual string GUINbr { get; set; }
        public abstract class gUINbr : PX.Data.BQL.BqlString.Field<gUINbr> { }
        #endregion

        #region SequenceNo
        [PXDBInt()]
        [PXUIField(DisplayName = "Sequence No")]
        public virtual int? SequenceNo { get; set; }
        public abstract class sequenceNo : PX.Data.BQL.BqlInt.Field<sequenceNo> { }
        #endregion

        #region GUIStatus
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "GUI Status")]
        [TWNStringList.TWNGUIStatus.List]
        public virtual string GUIStatus { get; set; }
        public abstract class gUIStatus : PX.Data.BQL.BqlString.Field<gUIStatus> { }
        #endregion

        #region GUIDirection
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "GUI Direction")]
        [TWNStringList.TWNGUIDirection.List]
        public virtual string GUIDirection { get; set; }
        public abstract class gUIDirection : PX.Data.BQL.BqlString.Field<gUIDirection> { }
        #endregion

        #region GUIControlCode
        [PXDBString(14, IsUnicode = true)]
        [PXUIField(DisplayName = "GUI Control Code", Visible = false)]
        public virtual string GUIControlCode { get; set; }
        public abstract class gUIControlCode : PX.Data.BQL.BqlString.Field<gUIControlCode> { }
        #endregion    

        #region GUIDate
        [PXDBDateAndTime(UseTimeZone = true)]
        [PXUIField(DisplayName = "GUI Date")]
        public virtual DateTime? GUIDate { get; set; }
        public abstract class gUIDate : PX.Data.BQL.BqlDateTime.Field<gUIDate> { }
        #endregion

        #region GUIDecPeriod
        [PXDBDateAndTime(UseTimeZone = true)]
        [PXUIField(DisplayName = "GUI Declaration Period")]
        public virtual DateTime? GUIDecPeriod { get; set; }
        public abstract class gUIDecPeriod : PX.Data.BQL.BqlDateTime.Field<gUIDecPeriod> { }
        #endregion

        #region GUITitle
        [PXDBString(80, IsUnicode = true)]
        [PXUIField(DisplayName = "GUI Title")]
        public virtual string GUITitle { get; set; }
        public abstract class gUITitle : PX.Data.BQL.BqlString.Field<gUITitle> { }
        #endregion

        #region TaxZoneID
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Zone", Visibility = PXUIVisibility.Visible)]
        [PXSelector(typeof(TaxZone.taxZoneID), 
                    DescriptionField = typeof(TaxZone.descr), 
                    Filterable = true)]
        [PXRestrictor(typeof(Where<TaxZone.isManualVATZone, Equal<False>>), Messages.CantUseManualVAT)]
        public virtual string TaxZoneID { get; set; }
        public abstract class taxZoneID : PX.Data.BQL.BqlString.Field<taxZoneID> { }
        #endregion

        #region TaxCategoryID
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(TaxCategory.taxCategoryID), 
                    DescriptionField = typeof(TaxCategory.descr))]
        [PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), Messages.InactiveTaxCategory, 
                      typeof(TaxCategory.taxCategoryID))]
        public virtual string TaxCategoryID { get; set; }
        public abstract class taxCategoryID : PX.Data.BQL.BqlString.Field<taxCategoryID> { }
        #endregion

        #region TaxID
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax ID", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Tax.taxID), 
                    DescriptionField = typeof(Tax.descr))]
        public virtual string TaxID { get; set; }
        public abstract class taxID : PX.Data.BQL.BqlString.Field<taxID> { }
        #endregion

        #region VATType
        [PXDBString(2, IsUnicode = true)]
        [PXUIField(DisplayName = "VAT Type")]
        [TWNStringList.TWNGUIVATType.List]
        public virtual string VATType { get; set; }
        public abstract class vATType : PX.Data.BQL.BqlString.Field<vATType> { }
        #endregion

        #region TaxNbr
        [Descriptor.TaxNbrVerify(8, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Nbr.")]    
        public virtual string TaxNbr { get; set; }
        public abstract class taxNbr : PX.Data.BQL.BqlString.Field<taxNbr> { }
        #endregion

        #region OurTaxNbr
        [TaxNbrVerify(8, IsUnicode = true)]
        [PXUIField(DisplayName = "Our Tax Nbr.")]
        public virtual string OurTaxNbr { get; set; }
        public abstract class ourTaxNbr : PX.Data.BQL.BqlString.Field<ourTaxNbr> { }
        #endregion

        #region NetAmount
        [TWNetAmount(0)]
        [PXUIField(DisplayName = "Net Amount")]
        public virtual decimal? NetAmount { get; set; }
        public abstract class netAmount : PX.Data.BQL.BqlDecimal.Field<netAmount> { }
        #endregion

        #region TaxAmount
        [TWTaxAmount(0)]
        [PXUIField(DisplayName = "Tax Amount")]
        public virtual decimal? TaxAmount { get; set; }
        public abstract class taxAmount : PX.Data.BQL.BqlDecimal.Field<taxAmount> { }
        #endregion

        #region NetAmtRemain
        [TWNetAmount(0)]
        [PXUIField(DisplayName = "Net Amt Remain")]
        public virtual decimal? NetAmtRemain { get; set; }
        public abstract class netAmtRemain : PX.Data.BQL.BqlDecimal.Field<netAmtRemain> { }
        #endregion

        #region TaxAmtRemain
        [TWTaxAmount(0)]
        [PXUIField(DisplayName = "Tax Amt Remain")]
        public virtual decimal? TaxAmtRemain { get; set; }
        public abstract class taxAmtRemain : PX.Data.BQL.BqlDecimal.Field<taxAmtRemain> { }
        #endregion

        #region CustVend
        [PXDBString(20, IsUnicode = true)]
        [PXUIField(DisplayName = "Customer/Vendor")]
        [MultiBAccountSelctorRaw()]
        public virtual string CustVend { get; set; }
        public abstract class custVend : PX.Data.BQL.BqlString.Field<custVend> { }
        #endregion

        #region CustVendName
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Cust/Vend Name", Visible = false)]
        public virtual string CustVendName { get; set; }
        public abstract class custVendName : PX.Data.BQL.BqlString.Field<custVendName> { }
        #endregion

        #region DeductionCode
        [PXDBString(1, IsUnicode = true)]
        [PXUIField(DisplayName = "Deduction Code")]
        [PXSelector(typeof(Search<CSAttributeDetail.valueID,
                                  Where<CSAttributeDetail.attributeID, Equal<TWNManualGUIAPBill.DeductionNameAtt>>>),
                    typeof(CSAttributeDetail.description),
                    DescriptionField = typeof(CSAttributeDetail.description))]
        public virtual string DeductionCode { get; set; }
        public abstract class deductionCode : PX.Data.BQL.BqlString.Field<deductionCode> { }
        #endregion

        #region BranchID
        [Branch()]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region BatchNbr
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Batch Nbr.")]
        [PXSelector(typeof(Search<Batch.batchNbr, Where<Batch.module, Equal<BatchModule.moduleAP>,
                                                        Or<Batch.module, Equal<BatchModule.moduleAR>>>>),
                    DescriptionField = typeof(Batch.description),
                    ValidateValue = false)]
        public virtual string BatchNbr { get; set; }
        public abstract class batchNbr : PX.Data.BQL.BqlString.Field<batchNbr> { }
        #endregion

        #region DocType
        [PXDBString(3, IsFixed = true)]
        [PXUIField(DisplayName = "Doc. Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = false, Visible = false)]
        public virtual string DocType { get; set; }
        public new abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
        #endregion

        #region OrderNbr
        [PXDBString(20, IsUnicode = true)]
        [PXUIField(DisplayName = "Order Nbr.", Enabled = false)]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
        #endregion

        #region TransDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Trans Date")]
        public virtual DateTime? TransDate { get; set; }
        public abstract class transDate : PX.Data.BQL.BqlDateTime.Field<transDate> { }
        #endregion

        #region PrintedDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Printed Date", Visible = false)]
        public virtual DateTime? PrintedDate { get; set; }
        public abstract class printedDate : PX.Data.BQL.BqlDateTime.Field<printedDate> { }
        #endregion

        #region CustomType
        [PXDBString(1, IsUnicode = true)]
        [PXUIField(DisplayName = "Custom Type")]
        [TWNStringList.TWNGUICustomType.List]
        public virtual string CustomType { get; set; }
        public abstract class customType : PX.Data.BQL.BqlString.Field<customType> { }
        #endregion

        #region ExportMethods
        [PXDBString(1, IsUnicode = true)]
        [PXUIField(DisplayName = "Export Methods")]
        [PXSelector(typeof(TWNExportMethods.exportMethod),
                    DescriptionField = typeof(TWNExportMethods.description))]
        public virtual string ExportMethods { get; set; }
        public abstract class exportMethods : PX.Data.BQL.BqlString.Field<exportMethods> { }
        #endregion

        #region ExportTicketType
        [PXDBString(2, IsUnicode = true)]
        [PXUIField(DisplayName = "Export Ticket Types")]
        [PXSelector(typeof(TWNExportTicketTypes.exportTicketType),
                    DescriptionField = typeof(TWNExportTicketTypes.description))]
        public virtual string ExportTicketType { get; set; }
        public abstract class exportTicketType : PX.Data.BQL.BqlString.Field<exportTicketType> { }
        #endregion

        #region ExportTicketNbr
        [PXDBString(14, IsUnicode = true)]
        [PXUIField(DisplayName = "Export Ticket Nbr.")]
        public virtual string ExportTicketNbr { get; set; }
        public abstract class exportTicketNbr : PX.Data.BQL.BqlString.Field<exportTicketNbr> { }
        #endregion

        #region ClearingDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Clearing Date", Visible = false)]
        public virtual DateTime? ClearingDate { get; set; }
        public abstract class clearingDate : PX.Data.BQL.BqlDateTime.Field<clearingDate> { }
        #endregion

        #region Remark
        [PXDBString(256, IsUnicode = true)]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion  

        #region EGUIExcluded
        [PXDBBool()]
        [PXUIField(DisplayName = "eGUI Excluded")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? EGUIExcluded { get; set; }
        public abstract class eGUIExcluded : PX.Data.BQL.BqlBool.Field<eGUIExcluded> { }
        #endregion

        #region EGUIExported
        [PXDBBool()]
        [PXUIField(DisplayName = "eGUI Exported")]
        public virtual bool? EGUIExported { get; set; }
        public abstract class eGUIExported : PX.Data.BQL.BqlBool.Field<eGUIExported> { }
        #endregion

        #region EGUIExportedDateTime
        [PXDBDate()]
        [PXUIField(DisplayName = "eGUI Exported On")]
        public virtual DateTime? EGUIExportedDateTime { get; set; }
        public abstract class eGUIExportedDateTime : PX.Data.BQL.BqlDateTime.Field<eGUIExportedDateTime> { }
        #endregion

        #region CarrierType
        [PXDBString(6, IsUnicode = true)]
        [PXUIField(DisplayName = "Carrier Type")]
        public virtual string CarrierType { get; set; }
        public abstract class carrierType : PX.Data.BQL.BqlString.Field<carrierType> { }
        #endregion

        #region CarrierID
        [PXDBString(64, IsUnicode = true)]
        [PXUIField(DisplayName = "Carrier ID")]
        public virtual string CarrierID { get; set; }
        public abstract class carrierID : PX.Data.BQL.BqlString.Field<carrierID> { }
        #endregion

        #region NPONbr
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "NPO Nbr", Visibility = PXUIVisibility.SelectorVisible, IsDirty = true)]
        [NPONbrSelector]
        public virtual string NPONbr { get; set; }
        public abstract class nPONbr : PX.Data.BQL.BqlString.Field<nPONbr> { }
        #endregion

        #region B2CPrinted
        [PXDBBool()]
        [PXUIField(DisplayName = "B2C Printed", Visible = false)]
        public virtual bool? B2CPrinted { get; set; }
        public abstract class b2CPrinted : PX.Data.BQL.BqlBool.Field<b2CPrinted> { }
        #endregion

        #region PrintCount
        [PXDBInt()]
        [PXUIField(DisplayName = "Print Count", Visible = false)]
        public virtual int? PrintCount { get; set; }
        public abstract class printCount : PX.Data.BQL.BqlInt.Field<printCount> { }
        #endregion

        #region QREncrypter
        [PXDBString(80, IsUnicode = true)]
        [PXUIField(DisplayName = "QR Encrypter", Visible = false)]
        public virtual string QREncrypter { get; set; }
        public abstract class qREncrypter : PX.Data.BQL.BqlString.Field<qREncrypter> { }
        #endregion

        #region AddressLine       
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "GUI Address")]
        [PXMassMergableField]
        [PXPersonalDataField]
        public virtual string AddressLine { get; set; }
        public abstract class addressLine : PX.Data.BQL.BqlString.Field<addressLine> { }
        #endregion

        #region AllowUpload
        [PXDBBool()]
        [PXUIField(DisplayName = "Allow Upload BankPro", Enabled = false)]
        [PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? AllowUpload { get; set; }
        public abstract class allowUpload : PX.Data.BQL.BqlBool.Field<allowUpload> { }
        #endregion

        #region CRMDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Credit Note Date")]
        public virtual DateTime? CRMDate { get; set; }
        public abstract class cRMDate : PX.Data.BQL.BqlDateTime.Field<cRMDate> { }
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
        [PXSearchable(SearchCategory.AP | SearchCategory.IN | SearchCategory.AR | SearchCategory.SO,
                      "GUI Number: {0}, {1}, {2}",
                      new Type[] { typeof(TWNGUITrans.gUINbr), typeof(TWNGUITrans.gUIFormatCode), typeof(TWNGUITrans.gUIStatus)},
                      new Type[] { typeof(TWNGUITrans.gUIFormatCode), typeof(TWNGUITrans.gUINbr), typeof(TWNGUITrans.orderNbr)},
                      NumberFields = new Type[] { typeof(TWNGUITrans.gUINbr) },
                      Line1Format = "{0} {1} {2}",
                      Line1Fields = new Type[] { typeof(TWNGUITrans.gUIDirection), typeof(TWNGUITrans.netAmount), typeof(TWNGUITrans.taxAmount) },
                      Line2Format = "{0}", 
                      Line2Fields = new Type[] { typeof(TWNGUITrans.gUITitle) }
                     )]
        [PXNote]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #region tstamp        
        [PXDBTimestamp]
        public virtual byte[] tstamp { get; set; }
        public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }
        #endregion

        #region ChineseAmt
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Chinese Amount", Visible = false)]
        public virtual string ChineseAmt
        {
            [PXDependsOnFields(typeof(netAmount), typeof(taxAmount))]
            get
            {
                return AmtInChinese(Convert.ToInt32(this.NetAmount + this.TaxAmount));
            }
        }
        public abstract class chineseAmt : PX.Data.BQL.BqlString.Field<chineseAmt> { }
        #endregion
    }

    public static class TWGUIFormatCode
    {
        public const string vATInCode21 = "21";
        public const string vATInCode22 = "22";
        public const string vATInCode23 = "23";
        public const string vATInCode24 = "24";
        public const string vATInCode25 = "25";
        public const string vATInCode26 = "26";
        public const string vATInCode27 = "27";
        public const string vATInCode28 = "28";
        public const string vATInCode29 = "29";
        public const string vATOutCode31 = "31";
        public const string vATOutCode32 = "32";
        public const string vATOutCode33 = "33";
        public const string vATOutCode34 = "34";
        public const string vATOutCode35 = "35";
        public const string vATOutCode36 = "36";
        public const string vATOutCode37 = "37";
    }
}