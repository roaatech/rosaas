using Roaa.Rosas.Application.Tenants.HealthCheckStatus.Settings;
using Roaa.Rosas.Domain.Entities.Management;
using System.Collections.Concurrent;

namespace Roaa.Rosas.Application.Tenants.BackgroundServices
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


        public void AddAvailableTenantTask(JobTask task)
        {
            AvailableTenantsTasks.Add(task);
            availableTenantsTasks.Add(task);
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
            InaccessibleTenantsTasks.Add(task);
            inaccessibleTenantsTasks.Add(task);
        }

        public void RemoveJobTask(params JobTask[] tasks)
        {
            foreach (var task in tasks)
            {
                if (!removedTasks.Any(x => x.Id == task.Id ||
                                              (x.TenantId == task.TenantId &&
                                               x.ProductId == task.ProductId &&
                                               x.Type == JobTaskType.Available)))
                {
                    removedTasks.Add(task);
                }



                if (task.Type == JobTaskType.Available)
                {

                    var removedTask = availableTenantsTasks.Where(x => x.TenantId == task.TenantId && x.ProductId == task.ProductId && x.Type == JobTaskType.Available).SingleOrDefault();

                    if (removedTask is not null)
                    {
                        availableTenantsTasks.Remove(removedTask);
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
