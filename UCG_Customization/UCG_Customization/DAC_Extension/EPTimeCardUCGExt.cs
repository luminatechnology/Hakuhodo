using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Data.BQL.Fluent;

namespace PX.Objects.EP
{
    public class EPTimeCardUCGExt : PXCacheExtension<EPTimeCard>
    {
        #region Sun
        [PXInt]
        [PXUIField(DisplayName = "Sun", Enabled = false)]
        [PXTimeList(30, 335, ExclusiveValues = false)]
        [PXUnboundDefault(typeof(
            SelectFrom<EPTimeCardSummary>
            .Where<EPTimeCardSummary.timeCardCD.IsEqual<EPTimeCard.timeCardCD.FromCurrent>>
            .AggregateTo<Sum<EPTimeCardSummary.sun>>
            .SearchFor<EPTimeCardSummary.sun>))]
        public virtual int? Sun { get; set; }
        public abstract class sun : PX.Data.BQL.BqlInt.Field<sun> { }
        #endregion

        #region Mon
        [PXInt]
        [PXUIField(DisplayName = "Mon", Enabled = false)]
        [PXTimeList(30, 335, ExclusiveValues = false)]
        [PXUnboundDefault(typeof(
            SelectFrom<EPTimeCardSummary>
            .Where<EPTimeCardSummary.timeCardCD.IsEqual<EPTimeCard.timeCardCD.FromCurrent>>
            .AggregateTo<Sum<EPTimeCardSummary.mon>>
            .SearchFor<EPTimeCardSummary.mon>))]
        public virtual int? Mon { get; set; }
        public abstract class mon : PX.Data.BQL.BqlInt.Field<mon> { }
        #endregion

        #region Tue
        [PXInt]
        [PXUIField(DisplayName = "Tue", Enabled = false)]
        [PXTimeList(30, 335, ExclusiveValues = false)]
        [PXUnboundDefault(typeof(
            SelectFrom<EPTimeCardSummary>
            .Where<EPTimeCardSummary.timeCardCD.IsEqual<EPTimeCard.timeCardCD.FromCurrent>>
            .AggregateTo<Sum<EPTimeCardSummary.tue>>
            .SearchFor<EPTimeCardSummary.tue>))]
        public virtual int? Tue { get; set; }
        public abstract class tue : PX.Data.BQL.BqlInt.Field<tue> { }
        #endregion

        #region Wed
        [PXInt]
        [PXUIField(DisplayName = "Wed", Enabled = false)]
        [PXTimeList(30, 335, ExclusiveValues = false)]
        [PXUnboundDefault(typeof(
            SelectFrom<EPTimeCardSummary>
            .Where<EPTimeCardSummary.timeCardCD.IsEqual<EPTimeCard.timeCardCD.FromCurrent>>
            .AggregateTo<Sum<EPTimeCardSummary.wed>>
            .SearchFor<EPTimeCardSummary.wed>))]
        public virtual int? Wed { get; set; }
        public abstract class wed : PX.Data.BQL.BqlInt.Field<wed> { }
        #endregion

        #region Thu
        [PXInt]
        [PXUIField(DisplayName = "Thu", Enabled = false)]
        [PXTimeList(30, 335, ExclusiveValues = false)]
        [PXUnboundDefault(typeof(
            SelectFrom<EPTimeCardSummary>
            .Where<EPTimeCardSummary.timeCardCD.IsEqual<EPTimeCard.timeCardCD.FromCurrent>>
            .AggregateTo<Sum<EPTimeCardSummary.thu>>
            .SearchFor<EPTimeCardSummary.thu>))]
        public virtual int? Thu { get; set; }
        public abstract class thu : PX.Data.BQL.BqlInt.Field<thu> { }
        #endregion

        #region Fri
        [PXInt]
        [PXUIField(DisplayName = "Fri", Enabled = false)]
        [PXTimeList(30, 335, ExclusiveValues = false)]
        [PXUnboundDefault(typeof(
            SelectFrom<EPTimeCardSummary>
            .Where<EPTimeCardSummary.timeCardCD.IsEqual<EPTimeCard.timeCardCD.FromCurrent>>
            .AggregateTo<Sum<EPTimeCardSummary.fri>>
            .SearchFor<EPTimeCardSummary.fri>))]
        public virtual int? Fri { get; set; }
        public abstract class fri : PX.Data.BQL.BqlInt.Field<fri> { }
        #endregion

        #region Sat
        [PXInt]
        [PXUIField(DisplayName = "Sat", Enabled = false)]
        [PXTimeList(30, 335, ExclusiveValues = false)]
        [PXUnboundDefault(typeof(
            SelectFrom<EPTimeCardSummary>
            .Where<EPTimeCardSummary.timeCardCD.IsEqual<EPTimeCard.timeCardCD.FromCurrent>>
            .AggregateTo<Sum<EPTimeCardSummary.sat>>
            .SearchFor<EPTimeCardSummary.sat>))]
        public virtual int? Sat { get; set; }
        public abstract class sat : PX.Data.BQL.BqlInt.Field<sat> { }
        #endregion
    }
}
