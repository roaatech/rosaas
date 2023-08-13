using Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus.Settings;
using Roaa.Rosas.Domain.Entities.Management;
using System.Collections.Concurrent;

namespace Roaa.Rosas.Application.Services.Management.Tenants.HealthCheckStatus
{
    public class BackgroundServicesStore
    {
        public string ProductTenantHealthStatusTableName { get; set; } = string.Empty;
        public BlockingCollection<JobTask> AvailableTenantsTasks { get; private set; } = new BlockingCollection<JobTask>();
        public BlockingCollection<JobTask> UnavailableTenantsTasks { get; private set; } = new BlockingCollection<JobTask>();
        public BlockingCollection<JobTask> InaccessibleTenantsTasks { get; private set; } = new BlockingCollection<JobTask>();
        public BlockingCollection<JobTask> InformerTasks { get; private set; } = new BlockingCollection<JobTask>();
        public List<TenantHealthCheckUrlModel> TenanstHealthCheckUrls { get; private set; } = new();
        public HealthCheckSettings Settings { get; private set; } = new();




        private List<JobTask> removedTasks = new List<JobTask>();

        private List<JobTask> availableTenantsTasks = new List<JobTask>();

        private List<JobTask> unavailableTenantsTasks = new List<JobTask>();

        private List<JobTask> inaccessibleTenantsTasks = new List<JobTask>();

        private List<JobTask> informerTasks = new List<JobTask>();

        private Dictionary<Guid, string> tenantsNames = new();

        public string GetTenantName(Guid tenantId)
        {
            return tenantsNames[tenantId];
        }

        public void AddTenantsNames(Guid tenantId, string tenantName)
        {
            if (!tenantsNames.TryGetValue(tenantId, out string val))
            {
                tenantsNames.Add(tenantId, tenantName);
            }
        }
        public void SetHealthCheckSettings(HealthCheckSettings settings)
        {
            Settings = settings;
        }

        public void RefillAvailableTenantTask()
        {
            availableTenantsTasks.ForEach(task => AvailableTenantsTasks.Add(task));
        }

        public void RefillUnavailableTenantTask()
        {
            unavailableTenantsTasks.ForEach(task => UnavailableTenantsTasks.Add(task));
        }

        public void RefillInaccessibleTenantTask()
        {
            inaccessibleTenantsTasks.ForEach(task => InaccessibleTenantsTasks.Add(task));
        }
        public void RefillInformerTask()
        {
            informerTasks.ForEach(task => InformerTasks.Add(task));
        }


        public void AddAvailableTenantTask(JobTask task, string tenantName)
        {
            AvailableTenantsTasks.Add(task);
            availableTenantsTasks.Add(task);

            if (!tenantsNames.TryGetValue(task.TenantId, out string val))
            {
                tenantsNames.Add(task.TenantId, tenantName);
            }
        }

        public void AddUnavailableTenantTask(JobTask task)
        {
            UnavailableTenantsTasks.Add(task);
            unavailableTenantsTasks.Add(task);
        }

        public void AddInaccessibleTenantTask(JobTask task)
        {
            InaccessibleTenantsTasks.Add(task);
            inaccessibleTenantsTasks.Add(task);
        }

        public void AddInformerTask(JobTask task)
        {
            InformerTasks.Add(task);
            informerTasks.Add(task);
        }

        public void RemoveJobTask(params JobTask[] tasks)
        {
            foreach (var task in tasks)
            {
                if (!removedTasks.Any(x => x.Id == task.Id ||
                                              x.TenantId == task.TenantId &&
                                               x.ProductId == task.ProductId &&
                                               x.Type == JobTaskType.Available))
                {
                    removedTasks.Add(task);
                }



                if (task.Type == JobTaskType.Available)
                {

                    var removedTasks = availableTenantsTasks.Where(x => x.TenantId == task.TenantId && x.ProductId == task.ProductId && x.Type == JobTaskType.Available).ToList();

                    if (removedTasks is not null)
                    {
                        removedTasks.ForEach(removedTask => availableTenantsTasks.Remove(removedTask));
                    }
                }
                else
                {
                    var removedTask = unavailableTenantsTasks.Where(x => x.Id == task.Id).SingleOrDefault();

                    if (removedTask is not null)
                    {
                        unavailableTenantsTasks.Remove(removedTask);
                    }


                    removedTask = inaccessibleTenantsTasks.Where(x => x.Id == task.Id).SingleOrDefault();

                    if (removedTask is not null)
                    {
                        inaccessibleTenantsTasks.Remove(removedTask);
                    }


                    removedTask = informerTasks.Where(x => x.Id == task.Id).SingleOrDefault();

                    if (removedTask is not null)
                    {
                        informerTasks.Remove(removedTask);

                        if (!availableTenantsTasks.Where(x => x.TenantId == task.TenantId && x.Type == JobTaskType.Available).Any() &&
                             !inaccessibleTenantsTasks.Where(x => x.TenantId == task.TenantId && x.Type == JobTaskType.Available).Any() &&
                             !informerTasks.Where(x => x.TenantId == task.TenantId && x.Type == JobTaskType.Available).Any()
                           )
                        {
                            if (tenantsNames.TryGetValue(task.TenantId, out string val))
                            {
                                tenantsNames.Remove(task.TenantId);
                            }
                        }
                    }
                }
            }
        }

        public void RemoveUnavailableTenantTask(JobTask task)
        {

            var removedTask = unavailableTenantsTasks.Where(x => x.TenantId == task.TenantId && x.ProductId == task.ProductId).SingleOrDefault();

            if (removedTask is not null)
            {
                unavailableTenantsTasks.Remove(removedTask);
            }
        }

        public void RemoveInaccessibleTenantsTasks(JobTask task)
        {

            var removedTask = inaccessibleTenantsTasks.Where(x => x.TenantId == task.TenantId && x.ProductId == task.ProductId).SingleOrDefault();

            if (removedTask is not null)
            {
                inaccessibleTenantsTasks.Remove(removedTask);
            }
        }


        public bool MakeSureIsNotRemoved(JobTask task)
        {
            JobTask? removedTask = null;

            if (task.Type == JobTaskType.Available)
            {
                removedTask = removedTasks.Where(x => x.TenantId == task.TenantId && x.ProductId == task.ProductId && x.Type == JobTaskType.Available).SingleOrDefault();
            }
            else
            {
                removedTask = removedTasks.Where(x => x.Id == task.Id).SingleOrDefault();
            }

            if (removedTask is not null)
            {
                removedTasks.Remove(removedTask);
                return false;
            }

            return true;
        }
    }

    public class TenantHealthCheckUrlModel
    {
        public Guid ProductId { get; set; }
        public Guid TenantId { get; set; }
        public string HealthCheckUrl { get; set; } = string.Empty;
    }
}
