using PX.Data;
using PX.Data.Update;
using PX.Objects.AP;
using System;

namespace UCG_Customization.Descriptor
{
    public class APReturnAmountAttribute : ProcedureAttribute
    {

        public enum Type
        {
            HEADER, DETAIL
        }

        private readonly Type _type;

        public APReturnAmountAttribute(Type type)
        {
            _procedureName = "P_APReturnAmount";
            _type = type;
            if (type == Type.HEADER)
            {
                _procedureParam = new ProcedureParam[] {
                     new ProcedureParam(SPParameterType.IN,"CompanyID",PXInstanceHelper.CurrentCompany),
                     new ProcedureParam(SPParameterType.IN,"RefNbr",typeof(APRegister.refNbr)),
                     new ProcedureParam(SPParameterType.IN,"DocType",typeof(APRegister.docType)),
                     new ProcedureParam(SPParameterType.OUT,"ReturnValue",0m),
                };
            }
            else if (type == Type.DETAIL)
            {
                _procedureParam = new ProcedureParam[] {
                     new ProcedureParam(SPParameterType.IN,"CompanyID",PXInstanceHelper.CurrentCompany),
                     new ProcedureParam(SPParameterType.IN,"RefNbr",typeof(APTran.refNbr)),
                     new ProcedureParam(SPParameterType.IN,"DocType",typeof(APTran.tranType)),
                     new ProcedureParam(SPParameterType.IN,"ProjectID",typeof(APTran.projectID)),
                     new ProcedureParam(SPParameterType.IN,"BranchID",typeof(APTran.branchID)),
                     new ProcedureParam(SPParameterType.IN,"TaskID",typeof(APTran.taskID)),
                     new ProcedureParam(SPParameterType.IN,"InventoryID",typeof(APTran.inventoryID)),
                     new ProcedureParam(SPParameterType.OUT,"ReturnValue",0m),
                };
            }

        }

        protected override bool PreCallSP(PXCache sender, object item)
        {
            string docType = null;
            if (_type == Type.HEADER)
            {
                docType = (item as APRegister)?.DocType;
            }
            else if (_type == Type.DETAIL)
            {
                docType = (item as APTran)?.TranType;
            }

            if (docType != APDocType.Invoice) return false;
            return true;
        }

    }

}
