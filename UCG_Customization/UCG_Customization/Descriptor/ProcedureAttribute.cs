using PX.Data;
using System;
using System.Collections.Generic;

namespace UCG_Customization.Descriptor
{
    public abstract class ProcedureAttribute : PXEventSubscriberAttribute, IPXRowPersistedSubscriber, IPXRowSelectingSubscriber
    {
        protected string _procedureName;
        protected ProcedureParam[] _procedureParam;

        public ProcedureAttribute()
        {
        }
        /// <summary>
        /// return value = Output index 0
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name=""></param>
        public ProcedureAttribute(string procedureName, ProcedureParam[] procedureParam)
        {
            _procedureName = procedureName;
            _procedureParam = procedureParam;
        }


        public virtual void RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
        {
            if (e.Row == null || sender.GetValue(e.Row, _FieldName) != null) return;
            SetValue(sender, e.Row);
        }

        public virtual void RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
        {
            SetValue(sender, e.Row);
        }

        protected void SetValue(PXCache sender, object item)
        {
            if (item == null && !PreCallSP(sender, item)) return;
            List<PXSPParameter> _params = new List<PXSPParameter>();
            for (int i = 0; i < _procedureParam.Length; i++)
            {
                object value = _procedureParam[i].value;
                if (value == null)
                {
                    //取得Field Type
                    Type _type = _procedureParam[i].paramField;
                    //取得Field Name
                    string fieldName = char.ToUpper(_type.Name[0]) + _type.Name.Substring(1);
                    value = sender.GetValue(item, fieldName);
                }

                switch (_procedureParam[i].type)
                {
                    case SPParameterType.IN:
                        _params.Add(new PXSPInParameter(_procedureParam[i].paramName, value));
                        break;
                    case SPParameterType.OUT:
                        _params.Add(new PXSPOutParameter(_procedureParam[i].paramName, value));
                        break;
                    case SPParameterType.IN_OUT:
                        _params.Add(new PXSPInOutParameter(_procedureParam[i].paramName, value));
                        break;
                }
            }
            //add CompanyID
            //_params.Add(new PXSPInParameter("CompanyID", PXInstanceHelper.CurrentCompany));


            object outValue = null;
            using (new PXConnectionScope())
            {
                var obj = PXDatabase.Execute(_procedureName, _params.ToArray());
                if (obj.Length > 0)
                {
                    outValue = obj[0];
                }
            }
            sender.SetValue(item, _FieldName, outValue);
        }

        /// <summary>
        /// return false then SP not work
        /// </summary>
        /// <returns></returns>
        protected abstract bool PreCallSP(PXCache sender, object item);
    }

    public class ProcedureParam
    {
        public readonly SPParameterType type;
        public readonly string paramName;
        public readonly Type paramField;
        public readonly object value;

        public ProcedureParam(SPParameterType type, string paramName, Type paramField)
        {
            this.type = type;
            this.paramName = paramName;
            this.paramField = paramField;
        }

        public ProcedureParam(SPParameterType type, string paramName, object value)
        {
            this.type = type;
            this.paramName = paramName;
            this.value = value;
        }

    }
}
