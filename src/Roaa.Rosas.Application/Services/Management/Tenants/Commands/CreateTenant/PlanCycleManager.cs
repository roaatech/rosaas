using Roaa.Rosas.Common.Utilities;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus
{
    public abstract class PlanCycleManager : Enumeration<PlanCycleManager, PlanCycle>
    {
        #region Props
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
        public abstract DateTime GetExpiryDate(DateTime startDate);
        #endregion



        #region inners  

        private sealed class DayPlanCycle : PlanCycleManager
        {
            #region Corts
            public DayPlanCycle() : base(PlanCycle.Day) { }
            #endregion 

            #region overrides  
            public override DateTime GetExpiryDate(DateTime startDate)
            {
                return startDate.AddDays(1);
            }
            #endregion
        }


        private sealed class WeekPlanCycle : PlanCycleManager
        {
            #region Corts
            public WeekPlanCycle() : base(PlanCycle.Week) { }
            #endregion 

            #region overrides  
            public override DateTime GetExpiryDate(DateTime startDate)
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
            public override DateTime GetExpiryDate(DateTime startDate)
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
            public override DateTime GetExpiryDate(DateTime startDate)
            {
                return startDate.AddYears(1);
            }
            #endregion
        }


        #endregion

    }
}
