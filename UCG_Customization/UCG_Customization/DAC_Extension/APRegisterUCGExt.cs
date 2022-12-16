using PX.Data;
using System.Collections.Generic;
using UCG_Customization.Descriptor;

namespace PX.Objects.AP
{
    public class APRegisterUCGExt : PXCacheExtension<PX.Objects.AP.APRegister>
    {
        #region UsrEPLentAP 
        [PXDBString(15)]
        [PXUIField(DisplayName = "UsrEPLentAP")]
        [PXDBDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string UsrEPLentAP { get; set; }
        public abstract class usrEPLentAP : PX.Data.BQL.BqlString.Field<usrEPLentAP> { }
        #endregion

        #region Unbound
        #region Return Amount
        [PXDecimal(0)]
        [PXUIField(DisplayName = "Return Amount",IsReadOnly = true)]
        [APReturnAmount(APReturnAmountAttribute.Type.HEADER)]
        public virtual decimal? ReturnAmount { get; set; }
        public abstract class returnAmount : PX.Data.BQL.BqlDecimal.Field<returnAmount> { }
        #endregion

        #region IsApproving 
        [PXBool]
        [PXUIField(DisplayName = "IsApproving")]
        public virtual bool? IsApproving { get; set; }
        public abstract class isApproving : PX.Data.BQL.BqlBool.Field<isApproving> { }
        #endregion
        #endregion

        [PXOverride]
        [PXDBString(1, IsFixed = true)]
        [PXDefault(APDocStatus.Hold)]
        [PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        [PXDependsOnFields(
            typeof(APRegister.voided),
            typeof(APRegister.hold),
            typeof(APRegister.scheduled),
            typeof(APRegister.released),
            typeof(APRegister.printed),
            typeof(APRegister.prebooked),
            typeof(APRegister.openDoc),
            typeof(APRegister.approved),
            typeof(APRegister.dontApprove),
            typeof(APRegister.rejected),
            typeof(APRegister.docType))]
        [APDocStatusExt.List]
        public virtual string Status { get; set; }

        #region APDocStatusExt
        public class APDocStatusExt {

            public static string[] Values = AppendArr(APDocStatus.Values, "A");
            public static string[] Labels = AppendArr(APDocStatus.Labels, "科目確認");

            public class ListAttribute : PXStringListAttribute {
                public ListAttribute():base(
                    Values,
                    Labels
                    ) { }
            }

            public static string[] AppendArr(string[] arr, params string[] strs)
            {
                List<string> values = new List<string>();
                foreach (var s in arr) values.Add(s);
                foreach (var s in strs) values.Add(s);
                return values.ToArray();
            }
        }
        #endregion
    }
}