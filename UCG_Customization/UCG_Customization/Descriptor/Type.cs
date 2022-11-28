using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCG_Customization.Descriptor
{
    public enum AmountType { 
        /// <summary>
        /// 成本預算費用
        /// </summary>
        BUDGET_EXPENSE,
        /// <summary>
        /// 已使用費用
        /// </summary>
        USED_EXPENSE,

    }

    public enum SPParameterType { 
        IN,OUT,IN_OUT
    }
}
