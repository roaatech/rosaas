using Roaa.Rosas.Common.Utilities;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Utilities
{
    public abstract class PlanCycleManager : Enumeration<PlanCycleManager, PlanCycle>
    {
        #region Props
        public static readonly PlanCycleManager Unlimited = new UnlimitedPlanCycle();
        public static readonly PlanCycleManager Custom = new CustomPlanCycle();
        public static readonly PlanCycleManager ThreeDays = new ThreeDaysPlanCycle();
        public static readonly PlanCycleManager Day = new DayPlanCycle();
        public static readonly PlanCycleManager Week = new WeekPlanCycle();
        public static readonly PlanCycleManager Month = new MonthPlanCycle();
        public static readonly PlanCycleManager Year = new YearPlanCycle();
        #endregion

        #region Corts
        protected PlanCycleManager(PlanCycle cycle) : base(cycle)
        {
        }
        #endregion

        #region abst   
        public abstract DateTime? CalculateExpiryDate(DateTime startDate, int? customPeriodInDays, int? trialPeriodInDays, TenancyType tenancyType);
        public abstract DateTime? CalculateExpiryDate(DateTime startDate, int? customPeriodInDays);
        #endregion



        #region inners  

        private sealed class DayPlanCycle : PlanCycleManager
        {
            #region Corts
            public DayPlanCycle() : base(PlanCycle.OneDay) { }
            #endregion 

            #region overrides  
            public override DateTime? CalculateExpiryDate(DateTime startDate, int? customPeriodInDays, int? trialPeriodInDays, TenancyType tenancyType)
            {
                return Calculate(startDate, customPeriodInDays);
            }
            public override DateTime? CalculateExpiryDate(DateTime startDate, int? customPeriodInDays)
            {
                return Calculate(startDate, customPeriodInDays);
            }

            private DateTime? Calculate(DateTime startDate, int? customPeriodInDays)
            {
                return startDate.AddDays(1);
            }
            #endregion
        }

        private sealed class ThreeDaysPlanCycle : PlanCycleManager
        {
            #region Corts
            public ThreeDaysPlanCycle() : base(PlanCycle.ThreeDays) { }
            #endregion 

            #region overrides  
            public override DateTime? CalculateExpiryDate(DateTime startDate, int? customPeriodInDays, int? trialPeriodInDays, TenancyType tenancyType)
            {
                return Calculate(startDate, customPeriodInDays);
            }
            public override DateTime? CalculateExpiryDate(DateTime startDate, int? customPeriodInDays)
            {
                return Calculate(startDate, customPeriodInDays);
            }
            private DateTime? Calculate(DateTime startDate, int? customPeriodInDays)
            {
                return startDate.AddDays(3);
            }
            #endregion
        }


        private sealed class WeekPlanCycle : PlanCycleManager
        {
            #region Corts
            public WeekPlanCycle() : base(PlanCycle.Week) { }
            #endregion 

            #region overrides  
            public override DateTime? CalculateExpiryDate(DateTime startDate, int? customPeriodInDays, int? trialPeriodInDays, TenancyType tenancyType)
            {
                return Calculate(startDate, customPeriodInDays);
            }
            public override DateTime? CalculateExpiryDate(DateTime startDate, int? customPeriodInDays)
            {
                return Calculate(startDate, customPeriodInDays);
            }
            private DateTime? Calculate(DateTime startDate, int? customPeriodInDays)
            {
                return startDate.AddDays(7);
            }
            #endregion
        }


        private sealed class MonthPlanCycle : PlanCycleManager
        {
            #region Corts
            public MonthPlanCycle() : base(PlanCycle.Month) { }
            #endregion 

            #region overrides  
            public override DateTime? CalculateExpiryDate(DateTime startDate, int? customPeriodInDays, int? trialPeriodInDays, TenancyType tenancyType)
            {
                if (tenancyType == TenancyType.Planed && trialPeriodInDays is not null && customPeriodInDays > 0)
                {
                    return startDate.AddDays(trialPeriodInDays.Value);
                }

                return Calculate(startDate, customPeriodInDays);
            }
            public override DateTime? CalculateExpiryDate(DateTime startDate, int? customPeriodInDays)
            {
                return Calculate(startDate, customPeriodInDays);
            }
            private DateTime? Calculate(DateTime startDate, int? customPeriodInDays)
            {
                return startDate.AddMonths(1);
            }
            #endregion
        }

        private sealed class YearPlanCycle : PlanCycleManager
        {
            #region Corts
            public YearPlanCycle() : base(PlanCycle.Year) { }
            #endregion 

            #region overrides  
            public override DateTime? CalculateExpiryDate(DateTime startDate, int? customPeriodInDays, int? trialPeriodInDays, TenancyType tenancyType)
            {
                if (tenancyType == TenancyType.Planed && trialPeriodInDays is not null && customPeriodInDays > 0)
                {
                    return startDate.AddDays(trialPeriodInDays.Value);
                }

                return Calculate(startDate, customPeriodInDays);
            }
            public override DateTime? CalculateExpiryDate(DateTime startDate, int? customPeriodInDays)
            {
                return Calculate(startDate, customPeriodInDays);
            }
            private DateTime? Calculate(DateTime startDate, int? customPeriodInDays)
            {
                return startDate.AddYears(1);
            }
            #endregion
        }


        private sealed class CustomPlanCycle : PlanCycleManager
        {
            #region Corts
            public CustomPlanCycle() : base(PlanCycle.Custom) { }
            #endregion 

            #region overrides  
            public override DateTime? CalculateExpiryDate(DateTime startDate, int? customPeriodInDays, int? trialPeriodInDays, TenancyType tenancyType)
            {
                if (tenancyType == TenancyType.Planed && trialPeriodInDays is not null && customPeriodInDays > 0)
                {
                    return startDate.AddDays(trialPeriodInDays.Value);
                }

                return Calculate(startDate, customPeriodInDays);
            }
            public override DateTime? CalculateExpiryDate(DateTime startDate, int? customPeriodInDays)
            {
                return Calculate(startDate, customPeriodInDays);
            }
            private DateTime? Calculate(DateTime startDate, int? customPeriodInDays)
            {
                if (!customPeriodInDays.HasValue)
                {
                    throw new ArgumentNullException("customPeriodInDays", "The [customPeriodInDays] property can't be null");
                }

                return startDate.AddDays(customPeriodInDays.Value);
            }
            #endregion
        }

        private sealed class UnlimitedPlanCycle : PlanCycleManager
        {
            #region Corts
            public UnlimitedPlanCycle() : base(PlanCycle.Unlimited) { }
            #endregion 

            #region overrides  
            public override DateTime? CalculateExpiryDate(DateTime startDate, int? customPeriodInDays, int? trialPeriodInDays, TenancyType tenancyType)
            {
                if (tenancyType == TenancyType.Planed && trialPeriodInDays is not null && customPeriodInDays > 0)
                {
                    return startDate.AddDays(trialPeriodInDays.Value);
                }

                return Calculate(startDate, customPeriodInDays);
            }
            public override DateTime? CalculateExpiryDate(DateTime startDate, int? customPeriodInDays)
            {
                return Calculate(startDate, customPeriodInDays);
            }
            private DateTime? Calculate(DateTime startDate, int? customPeriodInDays)
            {
                return null;
            }
            #endregion
        }

        #endregion

    }
}
