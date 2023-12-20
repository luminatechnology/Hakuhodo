using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PX.Objects.EP.TimeCardMaint;

namespace PX.Objects.EP
{
    public class EPTimeCardSummaryUCGExt : PXCacheExtension<EPTimeCardSummaryWithInfo>
    {
        #region StatusText
        [PXString]
        public virtual String StatusText { get; set; }
        public abstract class statusText : PX.Data.BQL.BqlString.Field<statusText> { }
        #endregion
    }
}
