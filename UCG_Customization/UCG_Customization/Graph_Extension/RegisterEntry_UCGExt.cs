using PX.Data;
using PX.Objects.GL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.PM
{
    public class RegisterEntry_UCGExt : PXGraphExtension<PX.Objects.CN.Compliance.PM.GraphExtensions.RegisterEntryExt, RegisterEntry>
    {
        protected virtual void _(Events.RowPersisting<PMRegister> e, PXRowPersisting baseMethod)
        {
            baseMethod.Invoke(e.Cache, e.Args);
            if (e.Cancel)
            {
                e.Cancel = false;
            }
        }

    }
}
