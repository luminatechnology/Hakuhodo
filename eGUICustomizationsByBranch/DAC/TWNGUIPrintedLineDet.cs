using System;
using eGUICustomizations.Graph;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.IN;

namespace eGUICustomizations.DAC
{
    [Serializable]
    [PXCacheName("GUI Printed Line Details")]
    public class TWNGUIPrintedLineDet : IBqlTable
    {
        #region GUINbr
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">aaaaaaaaaaaaa")]
        [PXUIField(DisplayName = "GUI Nbr.", Visible = false)]
        [PXDefault(typeof(TWNPrintedLineFilter.gUINbr))]
        public virtual string GUINbr { get; set; }
        public abstract class gUINbr : PX.Data.BQL.BqlString.Field<gUINbr> { }
        #endregion
    
        #region LineNbr
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Sort Nbr.")]
        [PXDefault(0)]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region GUIFormatcode
        [PXDBString(2, IsKey = true, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "GUI Format code", Visible = false)]
        [PXDefault(typeof(TWNPrintedLineFilter.gUIFormatcode))]
        public virtual string GUIFormatcode { get; set; }
        public abstract class gUIFormatcode : PX.Data.BQL.BqlString.Field<gUIFormatcode> { }
        #endregion

        #region RefNbr
        [PXDBString(15, IsKey = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Ref. Nbr.", Visible = false)]
        [PXDefault(typeof(TWNPrintedLineFilter.refNbr))]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region Descr
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Description")]
        public virtual string Descr { get; set; }
        public abstract class descr : PX.Data.BQL.BqlString.Field<descr> { }
        #endregion
    
        #region Qty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Qty")]
        public virtual decimal? Qty { get; set; }
        public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }
        #endregion

        #region UnitPrice
        [PXDBPriceCost]
        [PXUIField(DisplayName = "Unit Price")]
        public virtual decimal? UnitPrice { get; set; }
        public abstract class unitPrice : PX.Data.BQL.BqlDecimal.Field<unitPrice> { }
        #endregion
    
        #region Amount
        [PXDBCurrency(typeof(ARTran.curyInfoID), typeof(amount), BaseCalc = false)]
        [PXUIField(DisplayName = "Amount")]
        [PXFormula(typeof(Mult<qty, unitPrice>))]
        public virtual decimal? Amount { get; set; }
        public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
        #endregion
    
        #region Remark
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion
    
        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion
    
        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion
    
        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion
    
        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion
    
        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion
    
        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion
    
        #region NoteID
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion
    
        #region Tstamp
        [PXDBTimestamp()]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}