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

        Task<Workflow> GetNextStageAsync(ExpectedTenantResourceStatus expectedResourceStatus,
            TenantStatus currentStatus,
                                                 TenantStep currentStep,
                                                 UserType userType,
                                                 WorkflowAction action = WorkflowAction.Ok,
                                                 WorkflowTrack track = WorkflowTrack.Normal,
                                                 CancellationToken cancellationToken = default);

        Task<Workflow> GetNextStageAsync(ExpectedTenantResourceStatus expectedResourceStatus,
            TenantStatus currentStatus,
                                                 TenantStep currentStep,
                                                 TenantStatus nextStatus,
                                                 UserType userType,
                                                 WorkflowAction action = WorkflowAction.Ok,
                                                 WorkflowTrack track = WorkflowTrack.Normal,
                                                 CancellationToken cancellationToken = default);

        Task<List<Workflow>> GetNextStagesAsync(ExpectedTenantResourceStatus expectedResourceStatus,
            TenantStatus currentStatus,
                                                           TenantStep currentStep,
                                                           UserType userType,
                                                           WorkflowTrack track = WorkflowTrack.Normal,
                                                           CancellationToken cancellationToken = default);



        Task<List<Workflow>> GetAllStagesAsync(CancellationToken cancellationToken = default);


        Task<List<Workflow>> FindDuplicatesStagesAsync(CancellationToken cancellationToken = default);

    }


    public class TenantWorkflow : ITenantWorkflow
    {
        private readonly List<Workflow> _workflow;
        private readonly List<WorkflowEvent> _workflowEvents;
        private readonly List<StepStatus> _stepStatuses;

        private readonly List<UserType> _admins = new List<UserType> { UserType.SuperAdmin, UserType.ClientAdmin, UserType.ProductAdmin, UserType.TenantAdmin, };
        private readonly List<UserType> _externalSystem = new List<UserType> { UserType.ExternalSystem };
        private readonly List<UserType> _admins_and_externalSystem = new List<UserType> { UserType.SuperAdmin,
                                                                                          UserType.ClientAdmin,
                                                                                          UserType.ProductAdmin,
                                                                                          UserType.TenantAdmin,
                                                                                          UserType.ExternalSystem };

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
                     Status = TenantStatus.Inactive,
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
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.None,
                    CurrentStep = TenantStep.None,
                    CurrentStatus = TenantStatus.None,
                    OwnerTypes = _admins_and_externalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.RecordCreated,
                    Name = "Create Record",
                    Message = "ROSAS - created a tenant record in ROSAS's database",
                },

                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.None,
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.RecordCreated,
                    OwnerTypes = _admins_and_externalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.SendingCreationRequest,
                    Name = "Send Creation Request",
                    Message = "ROSAS - called the external system to create the tenant resources for it",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantCreationRequestEvent },
                },

                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.None,
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.RecordCreated,
                    OwnerTypes = new List<UserType>(){ UserType.RosasSystem },
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.SendingCreationRequest,
                    Name = "Send Creation Request",
                    Message = "ROSAS - called the external system to create the tenant resources for it",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantCreationRequestEvent },
                },

                new Workflow()
                {/************************************************************************************************************************************/
                    
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.None,
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.SendingCreationRequest,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.RecordCreated,
                    Message = "The external system failed to receive the request",
                },/***********************************************************************************************************************************/
               
                   new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.None,
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.SendingCreationRequest,
                    OwnerTypes = _admins_and_externalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.Creating,
                    Message = "The external system is creating the tenant resources",
                },

                   new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.None,
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.Creating,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.CreatedAsActive,
                    Message = "The external system created the tenant resources",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.TenantActivatedEvent },
                },


                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.CreatedAsActive,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.SendingDeactivationRequest,
                    Name = "Deactivate",
                    Message = "ROSAS - called the external system to deactivate the tenant resources",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeactivationRequestEvent },
                },

                  new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.CreatedAsActive,
                    OwnerTypes = new List<UserType>(){ UserType.RosasSystem },
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.SendingDeactivationRequest,
                    Name = "Deactivate",
                    Message = "ROSAS - called the external system to deactivate the tenant resources",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeactivationRequestEvent },
                },

                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.CreatedAsActive,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.SendingDeletionRequest,
                    Name = "Delete",
                    Message = "ROSAS - called the external system to delete the tenant resources",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeletionRequestEvent },
                },

                new Workflow()
                {/************************************************************************************************************************************/
                  
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.SendingDeactivationRequest,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.Failure,
                    Message = "The external system failed to receive the request",
                },/***********************************************************************************************************************************/
               

                  new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.Active,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.SendingDeactivationRequest,
                    Name = "Deactivate",
                    Message = "ROSAS - called the external system to deactivate the tenant resources",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeactivationRequestEvent },
                },

                //  new Workflow()
                //{/************************************************************************************************************************************/
                   
                //    CurrentStep = TenantStep.Deactivation,
                //    CurrentStatus = TenantStatus.SendingDeactivationRequest,
                //    OwnerType = UserType.ExternalSystem,
                //    Action = WorkflowAction.Cancel,
                //    NextStep = TenantStep.Activation,
                //    NextStatus = TenantStatus.Active,
                //    Message = "The external system failed to receive the request",
                //},/***********************************************************************************************************************************/
               

                  new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.SendingDeactivationRequest,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.Deactivating,
                    Message = "The external system is deactivating the tenant resources",
                },

                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.Deactivating,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.Inactive,
                    Message = "The external system deactivated the tenant",
                },

                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.Inactive,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.SendingActivationRequest,
                    Name = "Activate",
                    Message = "ROSAS - called the external system to activate the tenant resources",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantActivationRequestEvent },
                },

                  new Workflow()
                {/************************************************************************************************************************************/
                   
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.SendingActivationRequest,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.Failure,
                    Message = "The external system failed to receive the request",
                },/***********************************************************************************************************************************/
                
               
                 new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.SendingActivationRequest,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.Activating,
                    Message = "The external system is activating the tenant resources",
                },

                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.Activating,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.Active,
                    Message = "The external system activated the tenant",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.TenantActivatedEvent },
                },

                   new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.Active,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.SendingDeletionRequest,
                    Name = "Delete",
                    Message = "ROSAS - called the external system to delete the tenant resources",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeletionRequestEvent },
                },

                  new Workflow()
                {/************************************************************************************************************************************/
                   
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.SendingDeletionRequest,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.Failure,
                    Message = "The external system failed to receive the request",
                },/***********************************************************************************************************************************/
               

                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.Inactive,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.SendingDeletionRequest,
                    Name = "Delete",
                    Message = "ROSAS - called the external system to delete the tenant resources",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeletionRequestEvent },
                },

                 new Workflow()
                {/************************************************************************************************************************************/
                   
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.SendingDeletionRequest,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.Failure,
                    Message = "The external system failed to receive the request",
                },/***********************************************************************************************************************************/
               

                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.SendingDeletionRequest,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.Deleting,
                    Message = "The external system is deleting the tenant resources",
                },

                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.SendingDeletionRequest,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.Deleting,
                    Message = "The external system is deleting the tenant resources",
                },

                  new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.Deleting,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.Deleted,
                    Message = "The external system deleted the tenant",
                },

                  new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.Deleting,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.Deleted,
                    Message = "The external system deleted the tenant",
                },
                   

                  //-----------------------------------------------------------------------------------------------//

             


                 new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.None,
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.SendingCreationRequest,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.Creating,
                    Name = "Re-Send Creation Request",
                    Message = "",
                },




                 new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.SendingActivationRequest,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.Activating,
                    Name = "Re-Send Activation Request",
                    Message = "",
                },



                 new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.SendingDeactivationRequest,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.Deactivating,
                    Name = "Re-Send Deactivation Request",
                    Message = "",
                },




                 new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.SendingDeletionRequest,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.Deleting,
                    Name = "Re-Send Deletion Request",
                    Message = "",
                },


                 new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.SendingDeletionRequest,
                    OwnerTypes = _admins,
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
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.None,
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.Creating,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.Failure,
                    Message = "",
                },


                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.Activating,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.Failure,
                    Message = "",
                },


                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.Deactivating,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.Failure,
                    Message = "",
                },


                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.Deleting,
                    OwnerTypes =  _externalSystem,
                    Action = WorkflowAction.Cancel,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.Failure,
                    Message = "",
                },

                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.Deleting,
                    OwnerTypes = _externalSystem,
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
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.None,
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.SendingCreationRequest,
                    Name = "Re-Create",
                    Message = "",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantCreationRequestEvent },
                },


                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.SendingActivationRequest,
                    Name = "Re-Activate",
                    Message = "",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantActivationRequestEvent },
                },


                  new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.SendingDeactivationRequest,
                    Name = "Re-Deactivate",
                    Message = "",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeactivationRequestEvent },
                },


                  new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.SendingDeletionRequest,
                    Name = "Re-Delete",
                    Message = "",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeletionRequestEvent },
                },


                  new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.SendingDeletionRequest,
                    Name = "Re-Delete",
                    Message = "",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeletionRequestEvent },
                },

                  

                  
           //-----------------------------------------------------------------------------------------------//
             //------------------------------------------------------------------------------------------//
                 //----------------------------------------------------------------------------------//
                     //---------------------------------------------------------------------------//
                         //--------------------------------------------------------------------//
                             //-------------------------------------------------------------//
                                 //-----------------------------------------------------//
                                     //---------------------------------------------//
                                         //-------------------------------------//

                   
                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.Inactive,
                    Name = "Undo Activation",
                    Message = "",
                },


                  new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.Active,
                    Name = "Undo Deactivation",
                    Message = "",
                },


                  new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.Inactive,
                    Name = "Undo Deletion",
                    Message = "",
                },


                  new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.Active,
                    Name = "Undo Deletion",
                    Message = "",
                },

                  
           //-----------------------------------------------------------------------------------------------//
           //-----------------------------------------------------------------------------------------------//
           //-----------------------------------------------------------------------------------------------//
           //-----------------------------------------------------------------------------------------------//


                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.None,
                    CurrentStep = TenantStep.Creation,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Creation,
                    NextStatus = TenantStatus.CreatedAsActive,
                    Message = "",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.TenantActivatedEvent },
                },


                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.Active,
                    Message = "",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.TenantActivatedEvent },
                },


                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.Inactive,
                    Message = "",
                },


                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.Deleted,
                    Message = "",
                },

                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerTypes = _externalSystem,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.Deleted,
                    Message = "",
                },



           //-----------------------------------------------------------------------------------------------//
           //-----------------------------------------------------------------------------------------------//
           //-----------------------------------------------------------------------------------------------//
           //-----------------------------------------------------------------------------------------------//
           //-----------------------------------------------------------------------------------------------//



            

                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Activation,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.SendingDeletionRequest,
                    Name = "Delete",
                    Message = "",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeletionRequestEvent  },
                },


                  new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Deactivation,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deletion,
                    NextStatus = TenantStatus.SendingDeletionRequest,
                    Name = "Delete",
                    Message = "",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeletionRequestEvent },
                },


                  new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Active,
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Deactivation,
                    NextStatus = TenantStatus.SendingDeactivationRequest,
                    Name = "Deactivate",
                    Message = "",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantDeactivationRequestEvent },
                },


                new Workflow()
                {
                    ExpectedResourceStatus = ExpectedTenantResourceStatus.Inactive,
                    CurrentStep = TenantStep.Deletion,
                    CurrentStatus = TenantStatus.Failure,
                    OwnerTypes = _admins,
                    Action = WorkflowAction.Ok,
                    NextStep = TenantStep.Activation,
                    NextStatus = TenantStatus.SendingActivationRequest,
                    Name = "Activate",
                    Message = "",
                    Events = new List<WorkflowEventEnum>{ WorkflowEventEnum.SendingTenantActivationRequestEvent },
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

        public async Task<Workflow> GetNextStageAsync(ExpectedTenantResourceStatus expectedResourceStatus, TenantStatus currentStatus,
                                                             TenantStep currentStep,
                                                             UserType ownerType,
                                                             WorkflowAction action = WorkflowAction.Ok,
                                                             WorkflowTrack track = WorkflowTrack.Normal,
                                                             CancellationToken cancellationToken = default)
        {
            return _workflow.Where(x => x.ExpectedResourceStatus == expectedResourceStatus &&
                                        x.CurrentStatus == currentStatus &&
                                        x.CurrentStep == currentStep &&
                                        x.OwnerTypes.Contains(ownerType) &&
                                        x.Action == action &&
                                        x.Track == track).SingleOrDefault();
        }


        public async Task<Workflow> GetNextStageAsync(ExpectedTenantResourceStatus expectedResourceStatus, TenantStatus currentStatus,
                                                              TenantStep currentStep,
                                                              TenantStatus nextStatus,
                                                              UserType ownerType,
                                                              WorkflowAction action = WorkflowAction.Ok,
                                                              WorkflowTrack track = WorkflowTrack.Normal,
                                                              CancellationToken cancellationToken = default)
        {
            return _workflow.Where(x => x.ExpectedResourceStatus == expectedResourceStatus &&
                                        x.CurrentStatus == currentStatus &&
                                        x.CurrentStep == currentStep &&
                                        x.NextStatus == nextStatus &&
                                        x.OwnerTypes.Contains(ownerType) &&
                                        x.Action == action &&
                                        x.Track == track).SingleOrDefault();
        }

        public async Task<List<Workflow>> GetNextStagesAsync(ExpectedTenantResourceStatus expectedResourceStatus, TenantStatus currentStatus,
                                                                         TenantStep currentStep,
                                                                          UserType userType,
                                                                          WorkflowTrack track = WorkflowTrack.Normal,
                                                                          CancellationToken cancellationToken = default)
        {
            return _workflow.Where(x => x.ExpectedResourceStatus == expectedResourceStatus &&
                                        x.CurrentStatus == currentStatus &&
                                        x.CurrentStep == currentStep &&
                                        x.OwnerTypes.Contains(userType) &&
                                        x.Track == track).ToList();
        }
        public async Task<List<Workflow>> FindDuplicatesStagesAsync(CancellationToken cancellationToken = default)
        {

            var Keys = _workflow.GroupBy(i => new
            {
                i.ExpectedResourceStatus,
                i.CurrentStatus,
                i.CurrentStep,
                i.NextStatus,
                i.OwnerTypes,
                i.Action,
                i.Track
            })
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

            return Keys.Select(key => _workflow.Where(x => x.ExpectedResourceStatus == key.ExpectedResourceStatus &&
                                                    x.CurrentStatus == key.CurrentStatus &&
                                                    x.CurrentStep == key.CurrentStep &&
                                                    x.NextStatus == key.NextStatus &&
                                                     x.OwnerTypes.All(key.OwnerTypes.Contains) &&
                                                    x.Action == key.Action &&
                                                    x.Track == key.Track)
                                        .ToList())
                .SelectMany(x => x)
                .ToList();
        }
        public async Task<List<Workflow>> GetAllStagesAsync(CancellationToken cancellationToken = default)
        {
            return _workflow;
        }

    }
}