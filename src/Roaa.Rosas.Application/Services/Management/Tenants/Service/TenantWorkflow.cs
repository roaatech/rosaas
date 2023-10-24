using Newtonsoft.Json;
using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Enums;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Application.Services.Management.Tenants.Service
{

    public interface ITenantWorkflow
    {
        Task<StepStatus> GetStepStatusAsync(TenantStatus status, CancellationToken cancellationToken = default);
        Task<WorkflowEvent> GetWorkflowEventByIdAsync(WorkflowEventEnum friendlyId, CancellationToken cancellationToken = default);

        Task<Workflow> GetNextProcessActionAsync(TenantStatus currentStatus,
                                                 TenantStep currentStep,
                                                 UserType userType,
                                                 WorkflowAction action = WorkflowAction.Ok,
                                                 WorkflowTrack track = WorkflowTrack.Normal,
                                                 CancellationToken cancellationToken = default);

        Task<Workflow> GetNextProcessActionAsync(TenantStatus currentStatus,
                                                 TenantStep currentStep,
                                                 TenantStatus nextStatus,
                                                 UserType userType,
                                                 WorkflowAction action = WorkflowAction.Ok,
                                                 WorkflowTrack track = WorkflowTrack.Normal,
                                                 CancellationToken cancellationToken = default);

        Task<ICollection<Workflow>> GetProcessActionsAsync(TenantStatus currentStatus,
                                                           TenantStep currentStep,
                                                           UserType userType,
                                                           WorkflowTrack track = WorkflowTrack.Normal,
                                                           CancellationToken cancellationToken = default);
    }


    public class TenantWorkflow : ITenantWorkflow
    {
        private readonly List<Workflow> _workflow;
        private readonly List<WorkflowEvent> _workflowEvents;
        private readonly List<StepStatus> _stepStatuses;

        public TenantWorkflow()
        {
            var jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            _workflowEvents = new List<WorkflowEvent>
            {
                new WorkflowEvent()
                {
                    FriendlyId = WorkflowEventEnum.TenantActivatedEvent,
                    Type =  JsonConvert.SerializeObject(typeof(TenantActivatedEvent),jsonSettings),
                },
                new WorkflowEvent()
                {
                    FriendlyId = WorkflowEventEnum.SendingTenantCreationRequestEvent,
                    Type =  JsonConvert.SerializeObject(typeof(SendingTenantCreationRequestEvent),jsonSettings),
                },
                new WorkflowEvent()
                {
                    FriendlyId = WorkflowEventEnum.SendingTenantActivationRequestEvent,
                    Type =  JsonConvert.SerializeObject(typeof(SendingTenantActivationRequestEvent),jsonSettings),
                },
                new WorkflowEvent()
                {
                    FriendlyId = WorkflowEventEnum.SendingTenantDeactivationRequestEvent,
                    Type =  JsonConvert.SerializeObject(typeof(SendingTenantDeactivationRequestEvent),jsonSettings),
                },
                new WorkflowEvent()
                {
                    FriendlyId = WorkflowEventEnum.SendingTenantDeletionRequestEvent,
                    Type =  JsonConvert.SerializeObject(typeof(SendingTenantDeletionRequestEvent),jsonSettings),
                },
            };

            _stepStatuses = new List<StepStatus>
            {
                new StepStatus()
                {
                     Status = TenantStatus.None,
                     Step = TenantStep.None,
                },
                new StepStatus()
                {
                     Status = TenantStatus.RecordCreated,
                     Step = TenantStep.Creation,
                },
                new StepStatus()
                {
                     Status = TenantStatus.SendingCreationRequest,
                     Step = TenantStep.Creation,
                },
                new StepStatus()
                {
                     Status = TenantStatus.Creating,
                     Step = TenantStep.Creation,
                },
                new StepStatus()
                {
                     Status = TenantStatus.CreatedAsActive,
                     Step = TenantStep.Creation,
                },
                new StepStatus()
                {
                     Status = TenantStatus.SendingActivationRequest,
                     Step = TenantStep.Activation,
                },
                new StepStatus()
                {
                     Status = TenantStatus.Activating,
                     Step = TenantStep.Activation,
                },
                new StepStatus()
                {
                     Status = TenantStatus.Active,
                     Step = TenantStep.Activation,
                },
                new StepStatus()
                {
                     Status = TenantStatus.SendingDeactivationRequest,
                     Step = TenantStep.Deactivation,
                },
                new StepStatus()
                {
                     Status = TenantStatus.Deactivating,
                     Step = TenantStep.Deactivation,
                },
                new StepStatus()
                {
                     Status = TenantStatus.Deactive,
                     Step = TenantStep.Deactivation,
                },
                new StepStatus()
                {
                     Status = TenantStatus.SendingDeletionRequest,
                     Step = TenantStep.Deletion,
                },
                new StepStatus()
                {
                     Status = TenantStatus.Deleting,
                     Step = TenantStep.Deletion,
                },
                new StepStatus()
                {
                     Status = TenantStatus.Deleted,
                     Step = TenantStep.Deletion,
                },
                new StepStatus()
                {
                     Status = TenantStatus.Failure,
                     Step = null,
                },
            };

            _workflow = new List<Workflow>
            {
                new Workflow()
                {
                    CurrentStep = TenantStep.None,
                    CurrentStatus = TenantStatus.None,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.RecordCreated,
                    Name = "Create Record",
                    Message = "ROSAS - created a tenant record in ROSAS's database",
                },

                new Workflow()
                {
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.RecordCreated,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.SendingCreationRequest,
                    Name = "Send Creation Request",
                    Message = "ROSAS - called the external system to create the tenant resources for it",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantCreationRequestEvent },
                },

                new Workflow()
                {/************************************************************************************************************************************/
                    
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.SendingCreationRequest,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.RecordCreated,
                    Message = "The external system failed to receive the request",
                },/***********************************************************************************************************************************/
               
                   new Workflow()
                {
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.SendingCreationRequest,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.Creating,
                    Message = "The external system is creating the tenant resources",
                },

                   new Workflow()
                {
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.Creating,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.CreatedAsActive,
                    Message = "The external system created the tenant resources",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.TenantActivatedEvent },
                },


                new Workflow()
                {
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.CreatedAsActive,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.SendingDeactivationRequest,
                    Name = "Deactivate",
                    Message = "ROSAS - called the external system to deactivate the tenant resources",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeactivationRequestEvent },
                },


                new Workflow()
                {
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.CreatedAsActive,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.SendingDeletionRequest,
                    Name = "Delete",
                    Message = "ROSAS - called the external system to delete the tenant resources",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeletionRequestEvent },
                },

                new Workflow()
                {/************************************************************************************************************************************/
                  
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.SendingDeactivationRequest,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.CreatedAsActive,
                    Message = "The external system failed to receive the request",
                },/***********************************************************************************************************************************/
               

                  new Workflow()
                {
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.Active,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.SendingDeactivationRequest,
                    Name = "Deactivate",
                    Message = "ROSAS - called the external system to deactivate the tenant resources",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeactivationRequestEvent },
                },

                  new Workflow()
                {/************************************************************************************************************************************/
                   
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.SendingDeactivationRequest,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.Active,
                    Message = "The external system failed to receive the request",
                },/***********************************************************************************************************************************/
               

                  new Workflow()
                {
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.SendingDeactivationRequest,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.Deactivating,
                    Message = "The external system is deactivating the tenant resources",
                },

                new Workflow()
                {
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.Deactivating,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.Deactive,
                    Message = "The external system deactivated the tenant",
                },

                new Workflow()
                {
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.Deactive,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.SendingActivationRequest,
                    Name = "Activate",
                    Message = "ROSAS - called the external system to activate the tenant resources",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantActivationRequestEvent },
                },

                  new Workflow()
                {/************************************************************************************************************************************/
                   
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.SendingActivationRequest,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.Deactive,
                    Message = "The external system failed to receive the request",
                },/***********************************************************************************************************************************/
               


                 new Workflow()
                {
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.SendingActivationRequest,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.Activating,
                    Message = "The external system is activating the tenant resources",
                },

                new Workflow()
                {
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.Activating,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.Active,
                    Message = "The external system activated the tenant",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.TenantActivatedEvent },
                },

                   new Workflow()
                {
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.Active,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.SendingDeletionRequest,
                    Name = "Delete",
                    Message = "ROSAS - called the external system to delete the tenant resources",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeletionRequestEvent },
                },

                  new Workflow()
                {/************************************************************************************************************************************/
                   
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.SendingDeletionRequest,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.Active,
                    Message = "The external system failed to receive the request",
                },/***********************************************************************************************************************************/
               

                new Workflow()
                {
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.Deactive,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.SendingDeletionRequest,
                    Name = "Delete",
                    Message = "ROSAS - called the external system to delete the tenant resources",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeletionRequestEvent },
                },

                new Workflow()
                {/************************************************************************************************************************************/
                   
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.SendingDeletionRequest,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.Deactive,
                    Message = "The external system failed to receive the request",
                },/***********************************************************************************************************************************/
               

                new Workflow()
                {
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.SendingDeletionRequest,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.Deleting,
                    Message = "The external system is deleting the tenant resources",
                },
                  new Workflow()
                {
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.Deleting,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.Deleted,
                    Message = "The external system deleted the tenant",
                },




                  //-----------------------------------------------------------------------------------------------//

             


                 new Workflow()
                {
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.SendingCreationRequest,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.Creating,
                    Name = "Re-Send Creation Request",
                    Message = "",
                },




                 new Workflow()
                {
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.SendingActivationRequest,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.Activating,
                    Name = "Re-Send Activation Request",
                    Message = "",
                },



                 new Workflow()
                {
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.SendingDeactivationRequest,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.Deactivating,
                    Name = "Re-Send Deactivation Request",
                    Message = "",
                },




                 new Workflow()
                {
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.SendingDeletionRequest,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.Deleting,
                    Name = "Re-Send Deletion Request",
                    Message = "",
                },

           //-----------------------------------------------------------------------------------------------//
           //-----------------------------------------------------------------------------------------------//



                new Workflow()
                {
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.Creating,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.Failure,
                    Message = "",
                },


                new Workflow()
                {
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.Activating,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.Failure,
                    Message = "",
                },


                new Workflow()
                {
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.Deactivating,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.Failure,
                    Message = "",
                },


                new Workflow()
                {
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.Deleting,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.Failure,
                    Message = "",
                },
                

           //-----------------------------------------------------------------------------------------------//
           //-----------------------------------------------------------------------------------------------//
           //-----------------------------------------------------------------------------------------------//



                    new Workflow()
                {
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.SendingCreationRequest,
                    Name = "Re-Create",
                    Message = "",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantCreationRequestEvent },
                },


                new Workflow()
                {
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.SendingActivationRequest,
                    Name = "Re-Activate",
                    Message = "",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantActivationRequestEvent },
                },


                  new Workflow()
                {
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.SendingDeactivationRequest,
                    Name = "Re-Deactivate",
                    Message = "",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeactivationRequestEvent },
                },


                  new Workflow()
                {
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerType = UserType.SuperAdmin,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.SendingDeletionRequest,
                    Name = "Re-Delete",
                    Message = "",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeletionRequestEvent },
                },

                  
           //-----------------------------------------------------------------------------------------------//
           //-----------------------------------------------------------------------------------------------//
           //-----------------------------------------------------------------------------------------------//
           //-----------------------------------------------------------------------------------------------//


                new Workflow()
                {
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.CreatedAsActive,
                    Message = "",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.TenantActivatedEvent },
                },


                new Workflow()
                {
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.Active,
                    Message = "",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.TenantActivatedEvent },
                },


                new Workflow()
                {
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.Deactive,
                    Message = "",
                },


                new Workflow()
                {
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerType = UserType.ExternalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.Deleted,
                    Message = "",
                },

            };

        }
        public async Task<StepStatus> GetStepStatusAsync(TenantStatus status, CancellationToken cancellationToken = default)
        {
            return _stepStatuses.Where(x => x.Status == status).SingleOrDefault();
        }

        public async Task<WorkflowEvent> GetWorkflowEventByIdAsync(WorkflowEventEnum friendlyId, CancellationToken cancellationToken = default)
        {
            return _workflowEvents.Where(x => x.FriendlyId == friendlyId).SingleOrDefault();
        }

        public async Task<Workflow> GetNextProcessActionAsync(TenantStatus currentStatus,
                                                             TenantStep currentStep,
                                                             UserType ownerType,
                                                             WorkflowAction action = WorkflowAction.Ok,
                                                             WorkflowTrack track = WorkflowTrack.Normal,
                                                             CancellationToken cancellationToken = default)
        {
            return _workflow.Where(x => x.CurrentStatus == currentStatus &&
                                        x.CurrentStep == currentStep &&
                                        x.OwnerType == ownerType &&
                                        x.Action == action &&
                                        x.Track == track).SingleOrDefault();
        }


        public async Task<Workflow> GetNextProcessActionAsync(TenantStatus currentStatus,
                                                              TenantStep currentStep,
                                                              TenantStatus nextStatus,
                                                              UserType ownerType,
                                                              WorkflowAction action = WorkflowAction.Ok,
                                                              WorkflowTrack track = WorkflowTrack.Normal,
                                                              CancellationToken cancellationToken = default)
        {
            return _workflow.Where(x => x.CurrentStatus == currentStatus &&
                                        x.CurrentStep == currentStep &&
                                        x.NextStatus == nextStatus &&
                                        x.OwnerType == ownerType &&
                                        x.Action == action &&
                                        x.Track == track).SingleOrDefault();
        }
        //public async Task<Workflow> GetNextProcessActionAsync(TenantStatus currentStatus,
        //                                                     TenantStatus nextStatus, 
        //                                                     UserType ownerType,
        //                                                     WorkflowAction action = WorkflowAction.Ok,
        //                                                     WorkflowTrack track = WorkflowTrack.Normal)
        //{
        //    return _workflow.Where(p => p.CurrentStatus == currentStatus &&
        //                               p.NextStatus == nextStatus &&
        //                               p.OwnerType == ownerType &&
        //                               p.Action == action &&
        //                               p.Track == track).SingleOrDefault();
        //}

        public async Task<ICollection<Workflow>> GetProcessActionsAsync(TenantStatus currentStatus,
                                                                         TenantStep currentStep,
                                                                          UserType userType,
                                                                          WorkflowTrack track = WorkflowTrack.Normal,
                                                                          CancellationToken cancellationToken = default)
        {
            return _workflow.Where(x => x.CurrentStatus == currentStatus &&
                                        x.CurrentStep == currentStep &&
                                        x.OwnerType == userType &&
                                        x.Track == track).ToList();
        }

    }
}