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

        public virtual void _(Events.FieldSelecting<EPTimeCardSummaryWithInfo, EPTimeCardSummaryUCGExt.statusText> e)
        {
            if (e.Row == null) return;
            var query =
                new PXSelectGroupBy<EPTimeCardSummary,
                Where<EPTimeCardSummary.timeCardCD, Equal<Current<EPTimeCard.timeCardCD>>>,
                Aggregate<
                    Sum<EPTimeCardSummary.mon,
                    Sum<EPTimeCardSummary.tue,
                    Sum<EPTimeCardSummary.wed,
                    Sum<EPTimeCardSummary.thu,
                    Sum<EPTimeCardSummary.fri,
                    Sum<EPTimeCardSummary.sat,
                    Sum<EPTimeCardSummary.sun>>>>>>>>>(Base);
            var total = query.SelectSingle();

            e.ReturnValue = PXLocalizer.Localize(GetStatusText(total));
        }
        #endregion

        #region Method
        public virtual string GetStatusText(EPTimeCardSummary sum)
        {
            string mon = IntToTimeStr(sum.Mon ?? 0);
            string tue = IntToTimeStr(sum.Tue ?? 0);
            string wed = IntToTimeStr(sum.Wed ?? 0);
            string thu = IntToTimeStr(sum.Thu ?? 0);
            string fri = IntToTimeStr(sum.Fri ?? 0);
            string sat = IntToTimeStr(sum.Sat ?? 0);
            string sun = IntToTimeStr(sum.Sun ?? 0);
            return $"Daily time summary: Mon:{mon}, Tue:{tue}, Wed:{wed}, Thu:{thu}, Fri:{fri}, Sat:{sat}, Sun:{sun}";
        }

        public virtual string IntToTimeStr(int time)
        {
            int hh = time / 60;
            int mm = time % 60;
            return $"{hh.ToString().PadLeft(2, '0')}:{mm.ToString().PadLeft(2, '0')}";
        }
        #endregion
    }
}
