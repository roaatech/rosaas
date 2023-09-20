using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
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
        private readonly IWebHostEnvironment _environment;
        private readonly IRequestBroker _requestBroker;
        private readonly IRosasDbContext _dbContext;
        private string _tenantName = string.Empty;


        public ExternalSystemAPI(IRequestBroker requestBroker,
                                IWebHostEnvironment environment,
                                IRosasDbContext dbContext,
                                 ILogger<ExternalSystemAPI> logger)
        {
            _requestBroker = requestBroker;
            _environment = environment;
            _dbContext = dbContext;
            _logger = logger;
        }



        public async Task<Result<ExternalSystemResultModel<dynamic>>> CreateTenantAsync(ExternalSystemRequestModel<CreateTenantModel> model, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestModelAsync(model, model.TenantId, cancellationToken: cancellationToken);

            var result = await _requestBroker.PostAsync<dynamic, CreateTenantModel>(request, cancellationToken);

            return RetrieveResult(result, request.Uri);
        }


        public async Task<Result<ExternalSystemResultModel<dynamic>>> ActivateTenantAsync(ExternalSystemRequestModel<ActivateTenantModel> model, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestModelAsync(model, model.TenantId, cancellationToken: cancellationToken);

            var result = await _requestBroker.PostAsync<dynamic, ActivateTenantModel>(request, cancellationToken);

            return RetrieveResult(result, request.Uri);
        }


        public async Task<Result<ExternalSystemResultModel<dynamic>>> DeactivateTenantAsync(ExternalSystemRequestModel<DeactivateTenantModel> model, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestModelAsync(model, model.TenantId, cancellationToken: cancellationToken);

            var result = await _requestBroker.PostAsync<dynamic, DeactivateTenantModel>(request, cancellationToken);

            return RetrieveResult(result, request.Uri);
        }


        public async Task<Result<ExternalSystemResultModel<dynamic>>> DeleteTenantAsync(ExternalSystemRequestModel<DeleteTenantModel> model, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestModelAsync(model, model.TenantId, cancellationToken: cancellationToken);

            var result = await _requestBroker.PostAsync<dynamic, DeleteTenantModel>(request, cancellationToken);

            return RetrieveResult(result, request.Uri);
        }



        public async Task<Result<ExternalSystemResultModel<dynamic>>> InformTheTenantUnavailableAsync(ExternalSystemRequestModel<InformTheTenantUnavailableModel> model, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestModelAsync(model, model.TenantId, cancellationToken: cancellationToken);

            var result = await _requestBroker.PostAsync<dynamic, InformTheTenantUnavailableModel>(request, cancellationToken);

            return RetrieveResult(result, request.Uri);
        }

        public async Task<Result<ExternalSystemResultModel<dynamic>>> CheckTenantHealthStatusAsync(ExternalSystemRequestModel<CheckTenantHealthStatusModel> model, CancellationToken cancellationToken = default)
        {
            var request = await BuildRequestModelAsync(model, model.TenantId, model.Data.TenantName, cancellationToken: cancellationToken);

            var result = await _requestBroker.GetAsync<dynamic, CheckTenantHealthStatusModel>(request, cancellationToken);

            _tenantName = model.Data.TenantName;

            return RetrieveResult(result, request.Uri, true);
        }


        public async Task<RequestModel<T>> BuildRequestModelAsync<T>(ExternalSystemRequestModel<T> model, Guid tenantId, string? tenantName = null, CancellationToken cancellationToken = default)
        {
            string name = tenantName ?? string.Empty;
            var token = await GenerateTokenAsync(cancellationToken);
            var uri = model.BaseUrl.Replace("{tenantId}", tenantId.ToString())
                                   .Replace("{name}", name);
            return new RequestModel<T>
             (
                 uri: uri,
                 data: model.Data,
                 requestAuthorization: token.Data,
                 header: ("api-key", model.ApiKey)

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

        private Result<ExternalSystemResultModel<T>> RetrieveResult<T>(RequestResult<T> requestResult, string url, bool isCheckHealthStatus = false)
        {
            var data = new ExternalSystemResultModel<T>
            {
                DurationInMillisecond = requestResult.DurationInMillisecond,
                Url = url,
            };

            //// temp - for test operations
            //if (!_environment.IsProductionEnvironment() && isCheckHealthStatus)
            //{

            //    if (_tenantName.Contains("-x0"))
            //    {
            //        var res = Result<ExternalSystemResultModel<T>>.Fail("custom error");
            //        res.WithData(data);
            //        return res;
            //    }
            //    return Result<ExternalSystemResultModel<T>>.Successful(data);
            //}


            //return Result<ExternalSystemResultModel<T>>.Successful(data);


            if (requestResult.Success)
            {
                return Result<ExternalSystemResultModel<T>>.Successful(data);
            }

            var errors = requestResult.Errors.Select(x => x.Value.Select(val => MessageDetail.Error(val, x.Key))).SelectMany(x => x).ToList();

            var result = Result<ExternalSystemResultModel<T>>.Fail(errors);
            result.WithData(data);
            return result;
        }

    }

}