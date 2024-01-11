using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using static PX.Objects.EP.TimeCardMaint;

namespace PX.Objects.EP
{
    public class TimeCardMaintUCGExt : PXGraphExtension<TimeCardMaint>
    {
        #region Event
        public virtual void _(Events.RowSelected<EPTimeCard> e)
        {
            if (e.Row == null) return;
            SetSummary();
        }

        #endregion

        #region Method
        public virtual void SetSummary()
        {
            var doc = Base.Document.Current;
            var docExt = doc.GetExtension<EPTimeCardUCGExt>();
            int? mon = 0, tue = 0, wed = 0, thu = 0, fri = 0, sat = 0, sun = 0;
            foreach (EPTimeCardSummary sum in Base.Summary.Select())
            {
                mon += sum.Mon ?? 0;
                tue += sum.Tue ?? 0;
                wed += sum.Wed ?? 0;
                thu += sum.Thu ?? 0;
                fri += sum.Fri ?? 0;
                sat += sum.Sat ?? 0;
                sun += sum.Sun ?? 0;
            }
            docExt.Mon = mon;
            docExt.Tue = tue;
            docExt.Wed = wed;
            docExt.Thu = thu;
            docExt.Fri = fri;
            docExt.Sat = sat;
            docExt.Sun = sun;
        }
        #endregion
    }
}
