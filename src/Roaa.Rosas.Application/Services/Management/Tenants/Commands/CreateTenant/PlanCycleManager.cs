using Roaa.Rosas.Common.Utilities;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Commands.ChangeTenantStatus
{
    public abstract class PlanCycleManager : Enumeration<PlanCycleManager, Cycle>
    {
        #region Props
        public static readonly PlanCycleManager Week = new WeekPlanCycle();
        public static readonly PlanCycleManager Month = new MonthPlanCycle();
        public static readonly PlanCycleManager Year = new YearPlanCycle();
        #endregion

        #region Corts
        protected PlanCycleManager(Cycle cycle) : base(cycle)
        {
        }
        #endregion

        #region abst   
        public abstract DateTime GetExpiryDate(DateTime startDate);
        #endregion



        #region inners  

        private sealed class WeekPlanCycle : PlanCycleManager
        {
            #region Corts
            public WeekPlanCycle() : base(Cycle.Week) { }
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
            public MonthPlanCycle() : base(Cycle.Month) { }
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
            public YearPlanCycle() : base(Cycle.Year) { }
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
