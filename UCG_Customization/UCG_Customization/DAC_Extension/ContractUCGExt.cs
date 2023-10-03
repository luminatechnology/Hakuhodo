using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace PX.Objects.CT
{
    public class ContractUCGExt : PXCacheExtension<Contract>
    {
        #region UsrProjectisLocked
        [PXDBBool()]
        [PXUIField(DisplayName = "Project Is Locked")]
        public virtual bool? UsrProjectisLocked { get; set; }
        public abstract class usrProjectisLocked : PX.Data.BQL.BqlBool.Field<usrProjectisLocked> { }
        #endregion

    }
}
