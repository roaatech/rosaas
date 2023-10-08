using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Service
{

    public interface ITenantWorkflow
    {
        Task<Workflow> GetNextProcessActionAsync(TenantStatus currentStatus,
                                                                          UserType userType,
                                                                          WorkflowAction action = WorkflowAction.Ok,
                                                                          WorkflowTrack track = WorkflowTrack.Normal);

        Task<ICollection<Workflow>> GetNextProcessActionsAsync(List<TenantStatus> currentStatuses,
                                                                        UserType ownerType,
                                                                         WorkflowAction action = WorkflowAction.Ok,
                                                                        WorkflowTrack track = WorkflowTrack.Normal);

        Task<ICollection<Workflow>> GetNextProcessActionsAsync(List<TenantStatus> currentStatuses,
                                                          TenantStatus nextStatus,
                                                          UserType ownerType,
                                                          WorkflowAction action = WorkflowAction.Ok,
                                                          WorkflowTrack track = WorkflowTrack.Normal);

        Task<Workflow> GetNextProcessActionAsync(TenantStatus currentStatus,
                                                             TenantStatus nextStatus,
                                                             UserType ownerType,
                                                             WorkflowAction action = WorkflowAction.Ok,
                                                             WorkflowTrack track = WorkflowTrack.Normal);

        Task<ICollection<Workflow>> GetProcessActionsAsync(TenantStatus currentStatus,
                                                                          UserType userType,
                                                                          WorkflowTrack track = WorkflowTrack.Normal);
    }
    public class TenantWorkflow : ITenantWorkflow
    {
        private readonly List<Workflow> _workflow;

        public TenantWorkflow()
        {
            _workflow = new List<Workflow>
            {
                new Workflow()
                {
                    CurrentStatus = TenantStatus.None,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStatus = TenantStatus.RecordCreated,
                    Name = "Create Record",
                    Message = "ROSAS - created a tenant record in ROSAS's database",
                },

                new Workflow()
                {
                    CurrentStatus = TenantStatus.RecordCreated,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStatus = TenantStatus.PreCreating,
                    Name = "Create",
                    Message = "ROSAS - called the external system to create the tenant resources for it",
                },

                new Workflow()
                {/************************************************************************************************************************************/
                    
                    CurrentStatus = TenantStatus.PreCreating,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStatus = TenantStatus.RecordCreated,
                    Message = "The external system failed to receive the request",
                },/***********************************************************************************************************************************/
               
                   new Workflow()
                {
                    CurrentStatus = TenantStatus.PreCreating,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    NextStatus = TenantStatus.Creating,
                    Message = "The external system is creating the tenant resources",
                },

                   new Workflow()
                {
                    CurrentStatus = TenantStatus.Creating,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    NextStatus = TenantStatus.CreatedAsActive,
                    Message = "The external system created the tenant resources",
                },


                new Workflow()
                {
                    CurrentStatus = TenantStatus.CreatedAsActive,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStatus = TenantStatus.PreDeactivating,
                    Name = "Deactivate",
                    Message = "ROSAS - called the external system to deactivate the tenant resources",
                },


                new Workflow()
                {
                    CurrentStatus = TenantStatus.CreatedAsActive,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStatus = TenantStatus.PreDeleting,
                    Name = "Delete",
                    Message = "ROSAS - called the external system to delete the tenant resources",
                },

                new Workflow()
                {/************************************************************************************************************************************/
                  
                    CurrentStatus = TenantStatus.PreDeactivating,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStatus = TenantStatus.CreatedAsActive,
                    Message = "The external system failed to receive the request",
                },/***********************************************************************************************************************************/
               

                  new Workflow()
                {
                    CurrentStatus = TenantStatus.Active,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStatus = TenantStatus.PreDeactivating,
                    Name = "Deactivate",
                    Message = "ROSAS - called the external system to deactivate the tenant resources",
                },

                  new Workflow()
                {/************************************************************************************************************************************/
                    CurrentStatus = TenantStatus.PreDeactivating,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStatus = TenantStatus.Active,
                    Message = "The external system failed to receive the request",
                },/***********************************************************************************************************************************/
               

                  new Workflow()
                {
                    CurrentStatus = TenantStatus.PreDeactivating,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    NextStatus = TenantStatus.Deactivating,
                    Message = "The external system is deactivating the tenant resources",
                },

                new Workflow()
                {
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    CurrentStatus = TenantStatus.Deactivating,
                    NextStatus = TenantStatus.Deactive,
                    Message = "The external system deactivated the tenant",
                },

                new Workflow()
                {
                    CurrentStatus = TenantStatus.Deactive,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStatus = TenantStatus.PreActivating,
                    Name = "Activate",
                    Message = "ROSAS - called the external system to activate the tenant resources",
                },

                  new Workflow()
                {/************************************************************************************************************************************/
                   
                    CurrentStatus = TenantStatus.PreActivating,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStatus = TenantStatus.Deactive,
                    Message = "The external system failed to receive the request",
                },/***********************************************************************************************************************************/
               


                 new Workflow()
                {
                    CurrentStatus = TenantStatus.PreActivating,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    NextStatus = TenantStatus.Activating,
                    Message = "The external system is activating the tenant resources",
                },

                new Workflow()
                {
                    CurrentStatus = TenantStatus.Activating,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    NextStatus = TenantStatus.Active,
                    Message = "The external system activated the tenant",
                },

                   new Workflow()
                {
                    CurrentStatus = TenantStatus.Active,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStatus = TenantStatus.PreDeleting,
                    Name = "Delete",
                    Message = "ROSAS - called the external system to delete the tenant resources",
                },

                  new Workflow()
                {/************************************************************************************************************************************/
                   
                    CurrentStatus = TenantStatus.PreDeleting,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStatus = TenantStatus.Active,
                    Message = "The external system failed to receive the request",
                },/***********************************************************************************************************************************/
               

                new Workflow()
                {
                    CurrentStatus = TenantStatus.Deactive,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStatus = TenantStatus.PreDeleting,
                    Name = "Delete",
                    Message = "ROSAS - called the external system to delete the tenant resources",
                },

                new Workflow()
                {/************************************************************************************************************************************/
                   
                    CurrentStatus = TenantStatus.PreDeleting,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStatus = TenantStatus.Deactive,
                    Message = "The external system failed to receive the request",
                },/***********************************************************************************************************************************/
               

                new Workflow()
                {
                    CurrentStatus = TenantStatus.PreDeleting,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    NextStatus = TenantStatus.Deleting,
                    Message = "The external system is deleting the tenant resources",
                },
                  new Workflow()
                {
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    CurrentStatus = TenantStatus.Deleting,
                    NextStatus = TenantStatus.Deleted,
                    Message = "The external system deleted the tenant",
                },
            };

        }



        public async Task<Workflow> GetNextProcessActionAsync(TenantStatus currentStatus,
                                                                         UserType ownerType,
                                                                          WorkflowAction action = WorkflowAction.Ok,
                                                                         WorkflowTrack track = WorkflowTrack.Normal)
        {
            return _workflow.Where(p => p.CurrentStatus == currentStatus && p.OwnerType == ownerType && p.Action == action && p.Track == track).SingleOrDefault();
        }

        public async Task<ICollection<Workflow>> GetNextProcessActionsAsync(List<TenantStatus> currentStatuses,
                                                                        UserType ownerType,
                                                                         WorkflowAction action = WorkflowAction.Ok,
                                                                        WorkflowTrack track = WorkflowTrack.Normal)
        {
            return _workflow.Where(p => currentStatuses.Contains(p.CurrentStatus) && p.OwnerType == ownerType && p.Action == action && p.Track == track).ToList();
        }

        public async Task<ICollection<Workflow>> GetNextProcessActionsAsync(List<TenantStatus> currentStatuses,
                                                           TenantStatus nextStatus,
                                                           UserType ownerType,
                                                           WorkflowAction action = WorkflowAction.Ok,
                                                           WorkflowTrack track = WorkflowTrack.Normal)
        {
            return _workflow.Where(p => currentStatuses.Contains(p.CurrentStatus) &&
                                       p.NextStatus == nextStatus &&
                                       p.OwnerType == ownerType &&
                                       p.Action == action &&
                                       p.Track == track).ToList();
        }
        public async Task<Workflow> GetNextProcessActionAsync(TenantStatus currentStatus,
                                                             TenantStatus nextStatus,
                                                             UserType ownerType,
                                                             WorkflowAction action = WorkflowAction.Ok,
                                                             WorkflowTrack track = WorkflowTrack.Normal)
        {
            return _workflow.Where(p => p.CurrentStatus == currentStatus &&
                                       p.NextStatus == nextStatus &&
                                       p.OwnerType == ownerType &&
                                       p.Action == action &&
                                       p.Track == track).SingleOrDefault();
        }

        public async Task<ICollection<Workflow>> GetProcessActionsAsync(TenantStatus currentStatus,
                                                                          UserType userType,
                                                                          WorkflowTrack track = WorkflowTrack.Normal)
        {
            return _workflow.Where(p => p.CurrentStatus == currentStatus && p.OwnerType == userType && p.Track == track).ToList();
        }

    }

}
/*   
                    Super Admin
                    pre-creating                        11/8/22, 4:38 PM
                    ROSAS - Super Admin created a tenant record in ROSAS. 


                    Super Admin
                    pre-creating                        11/8/22, 4:38 PM
                    ROSAS - Super Admin called the external system ({name}) to create the tenant resources for it. 


                    OSOS
                    creating                            11/8/22, 4:38 PM
                    The external system ({name}) creates the tenant resources. 


                    OSOS
                    pre-creating                        11/8/22, 4:38 PM
                    The external system ({name}) failed to receive the request. 


                    OSOS
                    activ                               11/8/22, 4:38 PM
                    The external system created the tenant resources. 

            
                    Super Admin
                    activating                          11/8/22, 4:38 PM
                    ROSAS - Super Admin set the tenant's status as {status}.
 */