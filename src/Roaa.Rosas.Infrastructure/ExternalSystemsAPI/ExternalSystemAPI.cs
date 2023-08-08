using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Common.Models.ResponseMessages;
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
            var request = await BuildRequestModelAsync(model, model.Data.TenantId, cancellationToken);

            var result = await _requestBroker.PostAsync<dynamic, CreateTenantModel>(request, cancellationToken);

            return RetrieveResult(result, request.Uri);
        }


        public async Task<Result<ExternalSystemResultModel<dynamic>>> ActivateTenantAsync(ExternalSystemRequestModel<ActivateTenantModel> model, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestModelAsync(model, model.Data.TenantId, cancellationToken);

            var result = await _requestBroker.PutAsync<dynamic, ActivateTenantModel>(request, cancellationToken);

            return RetrieveResult(result, request.Uri);
        }


        public async Task<Result<ExternalSystemResultModel<dynamic>>> DeactivateTenantAsync(ExternalSystemRequestModel<DeactivateTenantModel> model, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestModelAsync(model, model.Data.TenantId, cancellationToken);

            var result = await _requestBroker.PutAsync<dynamic, DeactivateTenantModel>(request, cancellationToken);

            return RetrieveResult(result, request.Uri);
        }


        public async Task<Result<ExternalSystemResultModel<dynamic>>> DeleteTenantAsync(ExternalSystemRequestModel<DeleteTenantModel> model, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestModelAsync(model, model.Data.TenantId, cancellationToken);

            var result = await _requestBroker.DeleteAsync<dynamic, DeleteTenantModel>(request, cancellationToken);

            return RetrieveResult(result, request.Uri);
        }



        public async Task<Result<ExternalSystemResultModel<dynamic>>> InformTheTenantUnavailabilityAsync(ExternalSystemRequestModel<InformTenantAvailabilityModel> model, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestModelAsync(model, model.Data.TenantId, cancellationToken);

            var result = await _requestBroker.PostAsync<dynamic, InformTenantAvailabilityModel>(request, cancellationToken);

            return RetrieveResult(result, request.Uri);
        }

        public async Task<Result<ExternalSystemResultModel<dynamic>>> CheckTenantHealthStatusAsync(ExternalSystemRequestModel<CheckTenantAvailabilityModel> model, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestModelAsync(model, model.Data.TenantId, cancellationToken);

            var result = await _requestBroker.GetAsync<dynamic, CheckTenantAvailabilityModel>(request, cancellationToken);

            return RetrieveResult(result, request.Uri);
        }


        public async Task<RequestModel<T>> BuildRequestModelAsync<T>(ExternalSystemRequestModel<T> model, Guid? tenantId, CancellationToken cancellationToken = default)
        {
            string id = tenantId.HasValue ? tenantId.Value.ToString() : string.Empty;
            var token = await GenerateTokenAsync(cancellationToken);
            var uri = $"{model.BaseUrl.Replace("{tenantId}", tenantId.ToString())}";
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
        private void LogInformation(Guid tenantId, string uri)
        {
            _logger.LogInformation($"{{0}}: sent a request to call the external system API   (Url:{uri}) , with the tenant TenantId({tenantId}).", "ExternalSystemAPI");
        }

        private Result<ExternalSystemResultModel<T>> RetrieveResult<T>(RequestResult<T> requestResult, string url)
        {
            var data = new ExternalSystemResultModel<T>
            {
                DurationInMillisecond = requestResult.DurationInMillisecond,
                Url = url,
            };
            return Result<ExternalSystemResultModel<T>>.Successful(data);

            if (requestResult.Success)
            {
                Result<ExternalSystemResultModel<T>>.Successful(data);
            }

            var errors = requestResult.Errors.Select(x => x.Value.Select(val => MessageDetail.Error(val, x.Key))).SelectMany(x => x).ToList();

            Result<ExternalSystemResultModel<T>>.Fail(errors);
        }

    }

}