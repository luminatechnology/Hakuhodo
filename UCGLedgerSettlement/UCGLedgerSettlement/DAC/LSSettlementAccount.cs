using System;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.GL;

namespace UCGLedgerSettlement.DAC
{
    [Serializable]
    //[PXTable]
	//[PXBreakInheritance]
    [PXCacheName("Settlement Account")]
    public class LSSettlementAccount : PX.Data.IBqlTable //Account
    {
        #region Keys
        public class PK : PrimaryKeyOf<LSSettlementAccount>.By<accountID>
        {
            public static LSSettlementAccount Find(PXGraph graph, int? accountID) => FindBy(graph, accountID);
        }
        #endregion

        #region AccountID	
        //[Account(DisplayName = "Account", IsKey = true,
        //               Visibility = PXUIVisibility.Visible, 
        //               DescriptionField = typeof(Account.description), 
        //               AvoidControlAccounts = true)]
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Account ID")]
        [PXDimensionSelector(AccountAttribute.DimensionName, typeof(Account.accountID), typeof(Account.accountCD))]
		public virtual int? AccountID { get; set; }
		public abstract class accountID : PX.Data.BQL.BqlInt.Field<accountID> { }
		#endregion

		#region AccountCD
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Account CD", Enabled = false)]
		[PXDefault(typeof(Search<Account.accountCD, Where<Account.accountID, Equal<Current<accountID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(Default<accountID>))]
		public virtual string AccountCD { get; set; }
		public abstract class accountCD : PX.Data.BQL.BqlString.Field<accountCD> { }
		#endregion

		#region Type
		[PXDBString(1, IsUnicode = true, IsFixed = true)]
		[PXUIField(DisplayName = "Type", IsReadOnly = true)]
		[PXDefault(typeof(Search<Account.type, Where<Account.accountID, Equal<Current<accountID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(Default<accountID>))]
		public virtual string Type { get; set; }
		public abstract class type : PX.Data.BQL.BqlString.Field<type> { }
		#endregion

		#region Description
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible, IsReadOnly = true)]
		[PXDefault(typeof(Search<Account.description, Where<Account.accountID, Equal<Current<accountID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(Default<accountID>))]
		public virtual string Description { get; set; }
		public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion

        #region ChkReferenceOnMatch
        [PXDBBool]
        [PXUIField(DisplayName = "Check Reference")]
        public virtual bool? ChkReferenceOnMatch { get; set; }
        public abstract class chkReferenceOnMatch : PX.Data.BQL.BqlBool.Field<chkReferenceOnMatch> { }
        #endregion

        #region ChkProjectOnMatch
        [PXDBBool]
        [PXUIField(DisplayName = "Check Project")]
        [PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? ChkProjectOnMatch { get; set; }
        public abstract class chkProjectOnMatch : PX.Data.BQL.BqlBool.Field<chkProjectOnMatch> { }
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
