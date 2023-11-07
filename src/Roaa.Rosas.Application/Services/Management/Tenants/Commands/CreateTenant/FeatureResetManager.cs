using Roaa.Rosas.Common.Utilities;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus
{
    public abstract class FeatureResetManager : Enumeration<FeatureResetManager, FeatureReset>
    {
        #region Props

        public static readonly FeatureResetManager Daily = new DailyFeatureReset();
        public static readonly FeatureResetManager Never = new NeverFeatureReset();
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
            #endregion
        }



        private sealed class NeverFeatureReset : FeatureResetManager
        {
            #region Corts
            public NeverFeatureReset() : base(FeatureReset.NonResettable) { }
            #endregion 

            #region overrides  
            public override DateTime? GetExpiryDate(DateTime startDate)
            {
                return null;
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
            #endregion
        }


        #endregion

    }
}
