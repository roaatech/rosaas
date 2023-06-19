using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Extensions;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Models.ExternalSystems;
using Roaa.Rosas.RequestBroker;
using Roaa.Rosas.RequestBroker.Models;

namespace Roaa.Rosas.Application.ExternalSystemsAPI
{
    public class ExternalSystemAPI : IExternalSystemAPI
    {
        private readonly ILogger<ExternalSystemAPI> _logger;
        private readonly IRequestBroker _requestBroker;





        public ExternalSystemAPI(IRequestBroker requestBroker,
                                 ILogger<ExternalSystemAPI> logger)
        {
            _requestBroker = requestBroker;
            _logger = logger;
        }






        public async Task<Result<ExternalSystemResultModel<dynamic>>> CreateTenantAsync(ExternalSystemRequestModel<CreateTenantModel> model, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestModelAsync(model, ExternalSystemEndpoints.CreateTenantEndpoint, model.Data.TenantId, cancellationToken);

            var result = await _requestBroker.PostAsync<ExternalSystemResultModel<dynamic>, CreateTenantModel>(request);

            _logger.LogInformation($"sent request to call the external system(TenantId:{model.Data.TenantId}, Url:{request.Uri}) API, with the tenant TenantId({model.Data.TenantId}).");

            return result.GetResult();
        }


        public async Task<Result<ExternalSystemResultModel<dynamic>>> ActivateTenantAsync(ExternalSystemRequestModel<ActivateTenantModel> model, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestModelAsync(model, ExternalSystemEndpoints.ActivateTenantEndpoint, model.Data.TenantId, cancellationToken);

            var result = await _requestBroker.PutAsync<ExternalSystemResultModel<dynamic>, ActivateTenantModel>(request);

            return result.GetResult();
        }


        public async Task<Result<ExternalSystemResultModel<dynamic>>> DeactivateTenantAsync(ExternalSystemRequestModel<DeactivateTenantModel> model, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestModelAsync(model, ExternalSystemEndpoints.DeactivateTenantEndpoint, model.Data.TenantId, cancellationToken);

            var result = await _requestBroker.PutAsync<ExternalSystemResultModel<dynamic>, DeactivateTenantModel>(request);

            return result.GetResult();
        }


        public async Task<Result<ExternalSystemResultModel<dynamic>>> DeleteTenantAsync(ExternalSystemRequestModel<DeleteTenantModel> model, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestModelAsync(model, ExternalSystemEndpoints.DeleteTenantEndpoint, model.Data.TenantId, cancellationToken);

            var result = await _requestBroker.DeleteAsync<ExternalSystemResultModel<dynamic>, DeleteTenantModel>(request);

            return result.GetResult();
        }


        public async Task<RequestModel<T>> BuildRequestModelAsync<T>(ExternalSystemRequestModel<T> model, string endpoint, Guid? tenantId, CancellationToken cancellationToken = default)
        {
            string id = tenantId.HasValue ? tenantId.Value.ToString() : string.Empty;
            var token = await GenerateTokenAsync(cancellationToken);
            var uri = $"{model.BaseUrl}/{endpoint.Replace("{id}", tenantId.ToString())}";
            return new RequestModel<T>
             (
                 uri: uri,
                 data: model.Data,
                 requestAuthorization: token.Data
            );
        }

        private async Task<Result<RequestAuthorizationModel>> GenerateTokenAsync(CancellationToken cancellationToken = default)
        {
            string accessToken = "";
            return Result<RequestAuthorizationModel>.Successful(new RequestAuthorizationModel("Bearer", accessToken));
        }

    }
    public class ExternalSystemEndpoints
    {
        public const string CreateTenantEndpoint = "Rosas/v1/Tenants";
        public const string DeleteTenantEndpoint = "Rosas/v1/Tenants/{id}";
        public const string ActivateTenantEndpoint = "Rosas/v1/Tenants/{id}/status/active";
        public const string DeactivateTenantEndpoint = "Rosas/v1/Tenants/{id}/status/deactive";

    }

}