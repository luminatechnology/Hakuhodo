using System;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.TX;
using eGUICustomizations.DAC;

namespace eGUICustomizations.Descriptor
{
    #region ChineseAmountAttribute
    /// <summary>
    /// Convert numbers to Chinese tranditional word.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class ChineseAmountAttribute : PXStringAttribute, IPXFieldSelectingSubscriber
    {
        void IPXFieldSelectingSubscriber.FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            ARInvoice invoice = sender.Graph.Caches[typeof(ARInvoice)].Current as ARInvoice;

            if (invoice != null)
            {
                e.ReturnValue = ARInvoiceEntry_Extension.AmtInChinese((int)invoice.CuryDocBal);
            }
        }
    }
    #endregion

    #region GUINbrAutoNumAttribute
    public class GUINbrAutoNumberAttribute : AutoNumberAttribute
    {
        private Type _vATOutCodeField;
        private string[] _vATOutCodeValues;
        private Type[] _setupFields;
        private string[] _setupValues;

        private string _dateField;
        private Type _dateType;

        private string _numberingID;
        private DateTime? _dateTime;

        //private string _emptyDateMessage;

        public GUINbrAutoNumberAttribute(Type vATOutCodeField, Type dateField) : base(vATOutCodeField, dateField) { }

        public GUINbrAutoNumberAttribute(Type vATOutCodeField, Type dateField, string[] vATOutCodeValues, Type[] setupFields) : base(vATOutCodeField, dateField, vATOutCodeValues, setupFields)
        {
            _dateField = dateField.Name;
            _dateType = BqlCommand.GetItemType(dateField);

            _vATOutCodeField = vATOutCodeField;
            _vATOutCodeValues = vATOutCodeValues;
            _setupFields = setupFields;
        }

        public override void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            if ((e.Operation & PXDBOperation.Command) != PXDBOperation.Insert && 
                ((e.Operation & PXDBOperation.Command) == PXDBOperation.Update && string.IsNullOrEmpty((string)sender.GetValue(e.Row, _FieldName))) == false) { return; }

            getfields(sender, e.Row);

            //fix for manual non key numbering, broken when we started returning null instead of "" from getnewnumber
            if ((NotSetNumber = GetNewNumberSymbol()) == null && NullString == string.Empty) { return; }

            if ((NotSetNumber = GetNewNumberSymbol(_numberingID)) == NullString)
            {
                object keyValue = sender.GetValue(e.Row, _FieldName);

                Numberings items = PXDatabase.GetSlot<Numberings>(typeof(Numberings).Name, typeof(Numbering));

                if (items != null && keyValue != null)
                {
                    foreach (KeyValuePair<string, string> item in items.GetNumberings())
                    {
                        if (item.Value == (string)keyValue || GetNewNumberSymbol(item.Key) == (string)keyValue)
                        {
                            throw new PXException(PX.Objects.CS.Messages.DocumentNbrEqualNewSymbol, (string)keyValue);
                        }
                    }
                }
                return;
            }

            ///<remarks> Since the GUI date is used to control the number generation of the GUI nbr, comment out the date validation. </remarks>
            //if (_dateTime == null)
            //{
            //    Exception ex;

            //    if (!string.IsNullOrEmpty(_emptyDateMessage))
            //    {
            //        ex = new AutoNumberException(_emptyDateMessage);
            //    }
            //    else
            //    {
            //        ex = new AutoNumberException(PX.Objects.Common.Messages.MustHaveValue, (sender.GetStateExt(e.Row, _dateField) as PXFieldState)?.DisplayName ?? _dateField);
            //    }

            //    sender.RaiseExceptionHandling(_dateField, e.Row, null, ex);

            //    throw ex;
            //}

            if (_numberingID != null && _dateTime != null)
            {
                NewNumber = GetNextNumber(sender, e.Row, _numberingID, _dateTime, NewNumber, out LastNbr, out WarnNbr, out NumberingSEQ);

                if (NewNumber.CompareTo(WarnNbr) >= 0)
                {
                    PXUIFieldAttribute.SetWarning(sender, e.Row, _FieldName, PX.Objects.CS.Messages.WarningNumReached);
                }

                _KeyToAbort = sender.GetValue(e.Row, _FieldName);

                sender.SetValue(e.Row, _FieldName, NewNumber);
            }
            else if (string.IsNullOrEmpty(NewNumber = (string)sender.GetValue(e.Row, _FieldName)) || string.Equals(NewNumber, NotSetNumber))
            {
                throw new AutoNumberException(PX.Objects.CS.Messages.CantAutoNumberSpecific, _numberingID);
            }
        }

        public override void CacheAttached(PXCache sender)
        {
            base.CacheAttached(sender);

            _UserNumbering = new ObjectRef<bool?>();
            _NewSymbol = new ObjectRef<string>();
            _setupValues = new string[_setupFields.Length];

            sender.SetAutoNumber(_FieldName);

            bool IsKey;
            if (!(IsKey = sender.Keys.IndexOf(_FieldName) > 0))
            {
                foreach (PXEventSubscriberAttribute attr in sender.GetAttributesReadonly(_FieldName))
                {
                    if (attr is PXDBFieldAttribute && (IsKey = ((PXDBFieldAttribute)attr).IsKey)) { break; }
                }
            }

            if (!IsKey)
            {
                NullString = string.Empty;
                NullMode = NullNumberingMode.UserNumbering;
            }
            else
            {
                sender.Graph.RowSelected.AddHandler(sender.GetItemType(), Parameter_RowSelected);
                sender.Graph.CommandPreparing.AddHandler(sender.GetItemType(), _FieldName, Parameter_CommandPreparing);
            }
        }

        private void getfields(PXCache sender, object row)
        {
            PXCache cache;
            Type _setupType = null;
            string _setupField = null;
            BqlCommand _Select = null;

            _numberingID = null;

            if (_vATOutCodeField != null)
            {
                string doctypeValue = (string)sender.GetValue(row, _vATOutCodeField.Name);

                int i;
                if ((i = Array.IndexOf(_vATOutCodeValues, doctypeValue)) >= 0 && _setupValues[i] != null)
                {
                    _numberingID = _setupValues[i];
                }
                else if (i >= 0 && _setupFields[i] != null)
                {
                    if (typeof(IBqlSearch).IsAssignableFrom(_setupFields[i]))
                    {
                        _Select = BqlCommand.CreateInstance(_setupFields[i]);
                        _setupType = BqlCommand.GetItemType(((IBqlSearch)_Select).GetField());
                        _setupField = ((IBqlSearch)_Select).GetField().Name;
                    }
                    else if (_setupFields[i].IsNested && typeof(IBqlField).IsAssignableFrom(_setupFields[i]))
                    {
                        _setupField = _setupFields[i].Name;
                        _setupType = BqlCommand.GetItemType(_setupFields[i]);
                    }
                }
            }
            else if ((_numberingID = _setupValues[0]) != null)
            {
            }
            else if (typeof(IBqlSearch).IsAssignableFrom(_setupFields[0]))
            {
                _Select = BqlCommand.CreateInstance(_setupFields[0]);
                _setupType = BqlCommand.GetItemType(((IBqlSearch)_Select).GetField());
                _setupField = ((IBqlSearch)_Select).GetField().Name;
            }
            else if (_setupFields[0].IsNested && typeof(IBqlField).IsAssignableFrom(_setupFields[0]))
            {
                _setupField = _setupFields[0].Name;
                _setupType = BqlCommand.GetItemType(_setupFields[0]);
            }

            if (_Select != null)
            {
                PXView view = sender.Graph.TypedViews.GetView(_Select, false);
                int startRow = -1;
                int totalRows = 0;
                List<object> source = view.Select(new object[] { row }, null, null, null, null, null,
                                                  ref startRow, 1, ref totalRows);

                if (source != null && source.Count > 0)
                {
                    object item = source[source.Count - 1];
                    if (item != null && item is PXResult)
                    {
                        item = ((PXResult)item)[_setupType];
                    }
                    _numberingID = (string)sender.Graph.Caches[_setupType].GetValue(item, _setupField);
                }
            }
            else if (_setupType != null)
            {
                cache = sender.Graph.Caches[_setupType];
                if (cache.Current != null && _numberingID == null)
                {
                    _numberingID = (string)cache.GetValue(cache.Current, _setupField);
                }
            }

            cache = sender.Graph.Caches[_dateType];
            if (sender.GetItemType() == _dateType)
            {
                _dateTime = (DateTime?)cache.GetValue(row, _dateField);
            }
            else if (cache.Current != null)
            {
                _dateTime = (DateTime?)cache.GetValue(cache.Current, _dateField);
            }
        }

        public static new void SetNumberingId<Field>(PXCache cache, string value) where Field : IBqlField
        {
            foreach (PXEventSubscriberAttribute attr in cache.GetAttributesReadonly<Field>())
            {
                if (attr is GUINbrAutoNumberAttribute && attr.AttributeLevel == PXAttributeLevel.Cache && ((GUINbrAutoNumberAttribute)attr)._vATOutCodeValues.Length == 0)
                {
                    ((GUINbrAutoNumberAttribute)attr)._setupValues[0] = value;
                }
            }
        }

        protected static new string GetNextNumber(PXCache sender, object data, string numberingID, DateTime? dateTime)
        {
            string LastNbr, WarnNbr;
            int? NumberingSEQ;

            return GetNextNumber(sender, data, numberingID, dateTime, null, out LastNbr, out WarnNbr, out NumberingSEQ);
        }

        protected static new string GetNextNumber(PXCache sender, object data, string numberingID, DateTime? dateTime, string lastAssigned, out string LastNbr, out string WarnNbr, out int? NumberingSEQ)
        {
            if (IS_SEPARATE_SCOPE)
            {
                using (new PXConnectionScope())
                {
                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        PXTransactionScope.SetSuppressWorkflow(true);
                        string NewNumber = GetNextNumberInt(sender, data, numberingID, dateTime, lastAssigned, out LastNbr, out WarnNbr, out NumberingSEQ);
                        ts.Complete();
                        return NewNumber;
                    }
                }
            }
            else
            {
                return GetNextNumberInt(sender, data, numberingID, dateTime, lastAssigned, out LastNbr, out WarnNbr, out NumberingSEQ);
            }
        }

        protected static new string GetNextNumberInt(PXCache sender, object data, string numberingID, DateTime? dateTime, string lastAssigned, out string LastNbr, out string WarnNbr, out int? NumberingSEQ)
        {
            if (numberingID != null && dateTime != null)
            {
                int? branchID = sender.Graph.Accessinfo.BranchID;

                if (data != null && sender.Fields.Contains(PX.Objects.CM.CurrencyInfoAttribute.DefaultBranchIDFieldName))
                {
                    object state = sender.GetStateExt(data, PX.Objects.CM.CurrencyInfoAttribute.DefaultBranchIDFieldName);

                    if (state is PXFieldState && ((PXFieldState)state).Required == true)
                    {
                        branchID = (int?)sender.GetValue(data, PX.Objects.CM.CurrencyInfoAttribute.DefaultBranchIDFieldName);
                    }
                }

                NumberingSequence sequence = GetNumberingSequence(numberingID, branchID, dateTime);

                if (sequence == null) { throw new AutoNumberException(PX.Objects.CS.Messages.CantAutoNumberSpecific, numberingID); }

                LastNbr = sequence.LastNbr;
                WarnNbr = sequence.WarnNbr;
                NumberingSEQ = sequence.NumberingSEQ;

                string newNumber = NextNumber(LastNbr, sequence.NbrStep ?? 0);

                if (string.Equals(lastAssigned, newNumber, StringComparison.InvariantCultureIgnoreCase))
                {
                    newNumber = NextNumber(newNumber, sequence.NbrStep ?? 0);
                }
                // According to Kevin's request, the problem that the ending number cannot be obtained has been fixed.
                if (newNumber.CompareTo(sequence.EndNbr) > 0)//>= 0)
                {
                    throw new PXException(PX.Objects.CS.Messages.EndOfNumberingReached, numberingID);
                }

                try
                {
                    if (LastNbr != sequence.StartNbr)
                    {
                        if (!PXDatabase.Update<NumberingSequence>(new PXDataFieldAssign<NumberingSequence.lastNbr>(newNumber),
                                                                  new PXDataFieldRestrict<NumberingSequence.numberingID>(numberingID),
                                                                  new PXDataFieldRestrict<NumberingSequence.numberingSEQ>(NumberingSEQ),
                                                                  new PXDataFieldRestrict<NumberingSequence.lastNbr>(LastNbr),
                                                                  PXDataFieldRestrict.OperationSwitchAllowed))
                        {
                            PXDatabase.Update<NumberingSequence>(new PXDataFieldAssign<NumberingSequence.nbrStep>(sequence.NbrStep),
                                                                 new PXDataFieldRestrict<NumberingSequence.numberingID>(numberingID),
                                                                 new PXDataFieldRestrict<NumberingSequence.numberingSEQ>(NumberingSEQ));

                            using (PXDataRecord record = PXDatabase.SelectSingle<NumberingSequence>(new PXDataField<NumberingSequence.lastNbr>(),
                                                                                                    new PXDataFieldValue<NumberingSequence.numberingID>(numberingID),
                                                                                                    new PXDataFieldValue<NumberingSequence.numberingSEQ>(NumberingSEQ)))
                            {
                                if (record != null)
                                {
                                    LastNbr = record.GetString(0);
                                    newNumber = NextNumber(LastNbr, sequence.NbrStep ?? 0);
                                    // According to Kevin's request, the problem that the ending number cannot be obtained has been fixed.
                                    if (newNumber.CompareTo(sequence.EndNbr) > 0)//>= 0)
                                    {
                                        throw new PXException(PX.Objects.CS.Messages.EndOfNumberingReached, numberingID);
                                    }
                                }
                            }
                            PXDatabase.Update<NumberingSequence>(new PXDataFieldAssign<NumberingSequence.lastNbr>(newNumber),
                                                                 new PXDataFieldRestrict<NumberingSequence.numberingID>(numberingID),
                                                                 new PXDataFieldRestrict<NumberingSequence.numberingSEQ>(NumberingSEQ));
                        }
                    }
                    else
                    {
                        PXDatabase.Update<NumberingSequence>(new PXDataFieldAssign<NumberingSequence.lastNbr>(newNumber),
                                                             new PXDataFieldRestrict<NumberingSequence.numberingID>(numberingID),
                                                             new PXDataFieldRestrict<NumberingSequence.numberingSEQ>(NumberingSEQ),
                                                             PXDataFieldRestrict.OperationSwitchAllowed);
                    }
                }
                catch (PXDbOperationSwitchRequiredException)
                {
                    PXDatabase.Insert<NumberingSequence>(new PXDataFieldAssign<NumberingSequence.endNbr>(PXDbType.VarChar, 15, sequence.EndNbr),
                                                         new PXDataFieldAssign<NumberingSequence.lastNbr>(PXDbType.VarChar, 15, newNumber),
                                                         new PXDataFieldAssign<NumberingSequence.warnNbr>(PXDbType.VarChar, 15, sequence.WarnNbr),
                                                         new PXDataFieldAssign<NumberingSequence.nbrStep>(PXDbType.Int, 4, sequence.NbrStep ?? 0),
                                                         new PXDataFieldAssign<NumberingSequence.startNbr>(PXDbType.VarChar, 15, sequence.StartNbr),
                                                         new PXDataFieldAssign<NumberingSequence.startDate>(PXDbType.DateTime, sequence.StartDate),
                                                         new PXDataFieldAssign<NumberingSequence.createdByID>(PXDbType.UniqueIdentifier, 16, sequence.CreatedByID),
                                                         new PXDataFieldAssign<NumberingSequence.createdByScreenID>(PXDbType.Char, 8, sequence.CreatedByScreenID),
                                                         new PXDataFieldAssign<NumberingSequence.createdDateTime>(PXDbType.DateTime, 8, sequence.CreatedDateTime),
                                                         new PXDataFieldAssign<NumberingSequence.lastModifiedByID>(PXDbType.UniqueIdentifier, 16, sequence.LastModifiedByID),
                                                         new PXDataFieldAssign<NumberingSequence.lastModifiedByScreenID>(PXDbType.Char, 8, sequence.LastModifiedByScreenID),
                                                         new PXDataFieldAssign<NumberingSequence.lastModifiedDateTime>(PXDbType.DateTime, 8, sequence.LastModifiedDateTime),
                                                         new PXDataFieldAssign<NumberingSequence.numberingID>(PXDbType.VarChar, 10, numberingID),
                                                         new PXDataFieldAssign<NumberingSequence.nBranchID>(PXDbType.Int, 4, sequence.NBranchID));
                }

                return newNumber;
            }

            LastNbr = WarnNbr = null;
            NumberingSEQ = null;

            return null;
        }

        /// <summary>
        /// Specialized for ARRegister version of the <see cref="GUINbrAutoNumberAttribute"/><br/>
        /// It defines how the new numbers are generated for the GUI number. <br/>
        /// References ARRegisterExt.usrVATOutCode and ARRegisterExt.usrGUIDate fields of the document,<br/>
        /// and also define a link between numbering ID's defined in GUI steup and ARInvoice types:<br/>
        /// namely TWNGUIPreferences.gUI3CopiesManNumbering - for 31, 
        /// TWNGUIPreferences.gUI2CopiesNumbering - for 32<br/>        
        /// TWNGUIPreferences.gUI3CopiesNumbering - for 35 <br/>      
        /// </summary>
        public partial class NumberingAttribute : GUINbrAutoNumberAttribute
        {
            private static string[] _VATOutCodes
            {
                get
                {
                    return new string[] { TWGUIFormatCode.vATOutCode31, TWGUIFormatCode.vATOutCode32, TWGUIFormatCode.vATOutCode35 };
                }
            }

            private static Type[] _SetupFields
            {
                get
                {
                    return new Type[]
                    {
                    typeof(TWNGUIPreferences.gUI3CopiesManNumbering),
                    typeof(TWNGUIPreferences.gUI2CopiesNumbering),
                    typeof(TWNGUIPreferences.gUI3CopiesNumbering)
                    };
                }
            }

            public static Type GetNumberingIDField(string vATOutCode)
            {
                foreach (var pair in _VATOutCodes.Zip(_SetupFields))
                {
                    if (pair.Item1 == vATOutCode)
                    {
                        return pair.Item2;
                    }
                }

                return null;
            }

            public NumberingAttribute() : base(typeof(ARRegisterExt.usrVATOutCode), typeof(ARRegisterExt.usrGUIDate), _VATOutCodes, _SetupFields) { }

            protected NumberingAttribute(Type doctypeField, Type dateField, string[] doctypeValues, Type[] setupFields) : base(doctypeField, dateField, doctypeValues, setupFields) { }
        }
    }
    #endregion

    #region GUINumberAttribute
    public class GUINumberAttribute : PXDBStringAttribute/*, IPXFieldVerifyingSubscriber*/, IPXRowUpdatedSubscriber
    {
        public GUINumberAttribute(int length) : base(length) { }

        /// <summary>
        /// After trying for two hours, still can't get the current row record after entering the GUI number. It will be null. 
        /// Guess the GUI number is one of the PK then forced to change event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //public virtual void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        //{
        //    if (string.IsNullOrEmpty((string)e.NewValue) || TWNGUIValidation.ActivateTWGUI(new PXGraph()) == false) { return; }

        //    bool   reverse = false;
        //    string vATCode = null, erroMsg = null;

        //    switch (this.BqlTable.Name)
        //    {
        //        case nameof(ARRegister):
        //            vATCode = (string)sender.GetValue<ARRegisterExt.usrVATOutCode>(e.Row);
        //            reverse = sender.GetValue<ARRegister.docType>(e.Row).Equals(ARDocType.CreditMemo);
        //            break;
        //        case nameof(TWNGUITrans):
        //            vATCode = (string)sender.GetValue<TWNGUITrans.gUIFormatCode>(e.Row);
        //            break;
        //        case nameof(TWNManualGUIAP):
        //            vATCode = (string)sender.GetValue<TWNManualGUIAP.vATInCode>(e.Row);
        //            break;
        //        case nameof(TWNManualGUIAR):
        //            vATCode = (string)sender.GetValue<TWNManualGUIAR.vatOutCode>(e.Row);
        //            break;
        //        case nameof(TWNManualGUIBank):
        //            vATCode = (string)sender.GetValue<TWNManualGUIBank.vATInCode>(e.Row);
        //            break;
        //        case nameof(TWNManualGUIExpense):
        //            vATCode = (string)sender.GetValue<TWNManualGUIExpense.vATInCode>(e.Row);
        //            break;
        //        case nameof(TWNManualGUIAPBill):
        //            vATCode = (string)sender.GetValue<TWNManualGUIAPBill.vATInCode>(e.Row);
        //            break;
        //    }

        //    if (!vATCode.IsIn(TWGUIFormatCode.vATOutCode33, TWGUIFormatCode.vATOutCode34))
        //    {
        //        erroMsg = (vATCode.IsIn(TWGUIFormatCode.vATInCode21, TWGUIFormatCode.vATInCode23, TWGUIFormatCode.vATInCode25) && e.NewValue.ToString().Length != 10) ? TWMessages.GUINbrLength :
        //                                                                                                                                                                (e.NewValue.ToString().Length < 10) ? TWMessages.GUINbrMini :
        //                                                                                                                                                                                                      null;
        //    }

        //    if (!string.IsNullOrEmpty(erroMsg))
        //    {
        //        throw new PXSetPropertyException(erroMsg, PXErrorLevel.Error);
        //    }

        //    if (reverse == false)
        //    {
        //        new TWNGUIValidation().CheckGUINbrExisted(sender.Graph, (string)e.NewValue, vATCode);
        //    }
        //}
        public virtual void RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            string newGUINbr = (string)sender.GetValue(e.Row, "GUINbr");
            string oldGUINbr = (string)sender.GetValue(e.OldRow, "GUINbr");

            if (string.IsNullOrEmpty(newGUINbr) || 
                TWNGUIValidation.ActivateTWGUI(new PXGraph()) == false) { return; }

            bool   reverse = false;
            string vATCode = null, erroMsg = null;

            switch (this.BqlTable.Name)
            {
                case nameof(ARRegister):
                    reverse = sender.GetValue<ARRegister.docType>(e.Row).Equals(ARDocType.CreditMemo);

                    vATCode = (string)sender.GetValue<ARRegisterExt.usrVATOutCode>(e.Row);
                    break;
                case nameof(TWNGUITrans):
                    vATCode = (string)sender.GetValue<TWNGUITrans.gUIFormatCode>(e.Row);
                    break;
                case nameof(TWNManualGUIAP):
                    vATCode = (string)sender.GetValue<TWNManualGUIAP.vATInCode>(e.Row);
                    break;
                case nameof(TWNManualGUIAR):
                    vATCode = (string)sender.GetValue<TWNManualGUIAR.vatOutCode>(e.Row);
                    break;
                case nameof(TWNManualGUIBank):
                    vATCode = (string)sender.GetValue<TWNManualGUIBank.vATInCode>(e.Row);
                    break;
                case nameof(TWNManualGUIExpense):
                    vATCode = (string)sender.GetValue<TWNManualGUIExpense.vATInCode>(e.Row);
                    break;
                case nameof(TWNManualGUIAPBill):
                    vATCode = (string)sender.GetValue<TWNManualGUIAPBill.vATInCode>(e.Row);
                    break;
            }

            if (!vATCode.IsIn(TWGUIFormatCode.vATOutCode33, TWGUIFormatCode.vATOutCode34))
            {
                erroMsg = (vATCode.IsIn(TWGUIFormatCode.vATInCode21,
                                        TWGUIFormatCode.vATInCode23,
                                        TWGUIFormatCode.vATInCode25) && newGUINbr.Length != 10) ? TWMessages.GUINbrLength :
                                                                                                  (newGUINbr.Length < 10) ? TWMessages.GUINbrMini :
                                                                                                                            null;
            }

            if (!string.IsNullOrEmpty(erroMsg))
            {
                throw new PXSetPropertyException(erroMsg);
            }

            if (reverse == false && newGUINbr != oldGUINbr)
            {
                new TWNGUIValidation().CheckGUINbrExisted(sender.Graph, newGUINbr, vATCode);
            }
        }
    }
    #endregion

    #region TaxNbrVerifyAttribute
    public class TaxNbrVerifyAttribute : PXDBStringAttribute, IPXFieldVerifyingSubscriber
    {
        public TaxNbrVerifyAttribute(int length) : base(length) { }

        public void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if (e.NewValue != null)
            {
                string vATInCode = (string)sender.GetValue(e.Row, nameof(TWNGUITrans.GUIFormatCode));

                if (string.IsNullOrEmpty(vATInCode))
                {
                    vATInCode = (string)sender.GetValue(e.Row, nameof(TWNManualGUIAP.VATInCode));
                }

                ///<remarks>VAT in code = 27 匯總申報發票, Tax Nbr輸入四碼張數檢核</remarks>
                if (vATInCode != TWGUIFormatCode.vATInCode27)
                {
                    TWNGUIValidation validation = new TWNGUIValidation();

                    validation.CheckTabNbr(e.NewValue.ToString());

                    if (validation.errorOccurred == true)
                    {
                        //throw new PXSetPropertyException(validation.errorMessage, (PXErrorLevel)validation.errorLevel);
                        sender.RaiseExceptionHandling(this.FieldName, e.Row, e.NewValue,
                                                      new PXSetPropertyException(validation.errorMessage, (PXErrorLevel)validation.errorLevel));
                    }
                }
            }
        }
    }
    #endregion

    #region TWNetAmountAttribute
    public class TWNetAmountAttribute : PXDBDecimalAttribute, IPXFieldVerifyingSubscriber
    {
        public TWNetAmountAttribute(int percision) : base(percision) { }

        public void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if ((decimal)e.NewValue < 0)
            {
                // Throwing an exception to cancel assignment of the new value to the field
                throw new PXSetPropertyException(TWMessages.NetAmtNegError);
            }
        }
    }
    #endregion

    #region TWTaxAmountAttribute
    public class TWTaxAmountAttribute : PXDBDecimalAttribute, IPXFieldVerifyingSubscriber
    {
        protected Type _NetAmt;

        public TWTaxAmountAttribute(int percision) : base(percision) { }

        public TWTaxAmountAttribute(int percision, Type netAmt) : base(percision)
        {
            _NetAmt = netAmt;
        }

        public virtual void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if ((decimal)e.NewValue < 0)
            {
                throw new PXSetPropertyException(TWMessages.TaxAmtNegError);
            }

            if (_NetAmt != null)
            {
                var exception = new TWNGUIValidation().CheckTaxAmount(sender, (decimal)sender.GetValue(e.Row, _NetAmt.Name), (decimal)e.NewValue);

                if (exception != null)
                {
                    throw exception;
                }
            }
        }
    }
    #endregion

    #region TWTaxAmountCalcAttribute
    public class TWTaxAmountCalcAttribute : TWNetAmountAttribute, IPXFieldUpdatedSubscriber
    {
        protected Type _TaxID;
        protected Type _NetAmt;
        protected Type _TaxAmt;

        public TWTaxAmountCalcAttribute(int percision, Type taxID, Type netAmt, Type taxAmt) : base(percision)
        {
            _TaxID = taxID;
            _NetAmt = netAmt;
            _TaxAmt = taxAmt;
        }

        public virtual void FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            string taxID = (string)sender.GetValue(e.Row, _TaxID.Name);
            decimal netAmt = (decimal)sender.GetValue(e.Row, _NetAmt.Name);

            foreach (TaxRev taxRev in SelectFrom<TaxRev>.Where<TaxRev.taxID.IsEqual<@P.AsString>
                                                               .And<TaxRev.taxType.IsEqual<@P.AsString>>>.View.Select(sender.Graph, taxID, "P")) // P = Group type (Input)
            {
                decimal taxAmt = Math.Round(netAmt * (taxRev.TaxRate.Value / taxRev.NonDeductibleTaxRate.Value), 0);

                sender.SetValue(e.Row, _TaxAmt.Name, taxAmt);
            }
        }
    }
    #endregion

    #region TypeOfInSelectorAttribute
    public class TypeOfInSelectorAttribute : PXSelectorAttribute
    {
        public TypeOfInSelectorAttribute() : base(typeof(Search<CSAttributeDetail.valueID, Where<CSAttributeDetail.attributeID, Equal<TWNWHT.TypeOfInAtt>>>),
                                                  typeof(CSAttributeDetail.description))
        {
            Filterable = true;
            DirtyRead = true;
            DescriptionField = typeof(CSAttributeDetail.description);
        }
    }
    #endregion

    #region WHTFmtCodeSelectorAttribute
    public class WHTFmtCodeSelectorAttribute : PXSelectorAttribute
    {
        public WHTFmtCodeSelectorAttribute() : base(typeof(Search<CSAttributeDetail.valueID, Where<CSAttributeDetail.attributeID, Equal<TWNWHT.WHTFmtCodeAtt>>>),
                                                    typeof(CSAttributeDetail.description))
        {
            Filterable = true;
            DirtyRead = true;
            DescriptionField = typeof(CSAttributeDetail.description);
        }
    }
    #endregion

    #region WHTFmtSubSelectorAttribute
    public class WHTFmtSubSelectorAttribute : PXSelectorAttribute
    {
        public WHTFmtSubSelectorAttribute() : base(typeof(Search<CSAttributeDetail.valueID, Where<CSAttributeDetail.attributeID, Equal<TWNWHT.WHTFmtSubAtt>>>),
                                                   typeof(CSAttributeDetail.description))
        {
            Filterable = true;
            DirtyRead = true;
            DescriptionField = typeof(CSAttributeDetail.description);
        }
    }
    #endregion

    #region SecNHICodeSelectorAttribute
    public class SecNHICodeSelectorAttribute : PXSelectorAttribute
    {
        public SecNHICodeSelectorAttribute() : base(typeof(Search<CSAttributeDetail.valueID, Where<CSAttributeDetail.attributeID, Equal<TWNWHT.SecNHICodeAtt>>>),
                                                    typeof(CSAttributeDetail.description))
        {
            Filterable = true;
            DirtyRead = true;
            DescriptionField = typeof(CSAttributeDetail.description);
        }
    }
    #endregion

    #region MultiBAccountSelctorAttribute
    public class MultiBAccountSelectorAttribute : PXSelectorAttribute
    {
        public MultiBAccountSelectorAttribute() : base(typeof(Search<BAccount.bAccountID, Where<BAccount.type.IsIn<BAccountType.customerType,
                                                                                                                   BAccountType.vendorType,
                                                                                                                   BAccountType.combinedType>>>),
                                                       typeof(BAccount.acctCD))
        {
            SubstituteKey = typeof(BAccount.acctCD);
            Filterable = true;
            DirtyRead = true;
            DescriptionField = typeof(BAccount.acctName);
        }
    }
    #endregion

    #region MultiBAccountSelctorRawAttribute
    public class MultiBAccountSelctorRawAttribute : PXSelectorAttribute
    {
        public MultiBAccountSelctorRawAttribute() : base(typeof(Search<BAccount2.acctCD, Where<BAccount2.type.IsIn<BAccountType.customerType,
                                                                                                                   BAccountType.vendorType,
                                                                                                                   BAccountType.employeeType,
                                                                                                                   BAccountType.combinedType>>>),
                                                         typeof(BAccount2.acctName))
        {
            Filterable = true;
            DirtyRead = true;
            DescriptionField = typeof(BAccount2.acctName);
        }
    }
    #endregion
}