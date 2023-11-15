using Roaa.Rosas.Common.Utilities;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Utilities
{
    public abstract class FeatureResetManager : Enumeration<FeatureResetManager, FeatureReset>
    {
        #region Props

        public static readonly FeatureResetManager Daily = new DailyFeatureReset();
        public static readonly FeatureResetManager NonResettable = new NonResettableFeatureReset();
        public static readonly FeatureResetManager Weekly = new WeeklyFeatureReset();
        public static readonly FeatureResetManager Monthly = new MonthlyFeatureReset();
        public static readonly FeatureResetManager Annual = new AnnualFeatureReset();
        #endregion

        #region Corts
        protected FeatureResetManager(FeatureReset reset) : base(reset)
        {
        }
        #endregion

        #region abst   
        public abstract DateTime? GetExpiryDate(DateTime startDate);
        public abstract DateTime? GetStartDate(DateTime startDate);
        public abstract bool IsResettable();
        #endregion



        #region inners  
        private sealed class DailyFeatureReset : FeatureResetManager
        {
            #region Corts
            public DailyFeatureReset() : base(FeatureReset.Daily) { }
            #endregion 

            #region overrides  
            public override DateTime? GetExpiryDate(DateTime startDate)
            {
                return startDate.AddDays(1);
            }
            public override DateTime? GetStartDate(DateTime date)
            {
                return date;
            }
            public override bool IsResettable()
            {
                return true;
            }
            #endregion
        }



        private sealed class NonResettableFeatureReset : FeatureResetManager
        {
            #region Corts
            public NonResettableFeatureReset() : base(FeatureReset.NonResettable) { }
            #endregion 

            #region overrides  
            public override DateTime? GetExpiryDate(DateTime startDate)
            {
                return null;
            }
            public override DateTime? GetStartDate(DateTime date)
            {
                return null;
            }
            public override bool IsResettable()
            {
                return false;
            }
            #endregion
        }



        private sealed class WeeklyFeatureReset : FeatureResetManager
        {
            #region Corts
            public WeeklyFeatureReset() : base(FeatureReset.Weekly) { }
            #endregion 

            #region overrides  
            public override DateTime? GetExpiryDate(DateTime startDate)
            {
                return startDate.AddDays(7);
            }
            public override DateTime? GetStartDate(DateTime date)
            {
                return date;
            }
            public override bool IsResettable()
            {
                return true;
            }
            #endregion
        }


        private sealed class MonthlyFeatureReset : FeatureResetManager
        {
            #region Corts
            public MonthlyFeatureReset() : base(FeatureReset.Monthly) { }
            #endregion 

            #region overrides  
            public override DateTime? GetExpiryDate(DateTime startDate)
            {
                return startDate.AddMonths(1);
            }
            public override DateTime? GetStartDate(DateTime date)
            {
                return date;
            }
            public override bool IsResettable()
            {
                return true;
            }
            #endregion
        }


        private sealed class AnnualFeatureReset : FeatureResetManager
        {
            #region Corts
            public AnnualFeatureReset() : base(FeatureReset.Annual) { }
            #endregion 

            #region overrides  
            public override DateTime? GetExpiryDate(DateTime startDate)
            {
                return startDate.AddYears(1);
            }
            public override DateTime? GetStartDate(DateTime date)
            {
                return date;
            }
            public override bool IsResettable()
            {
                return true;
            }
            #endregion
        }


        #endregion

    }
}
