using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.ExternalSystemsAPI;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Domain.Models.Webhook;
using Roaa.Rosas.RequestBroker;
using Roaa.Rosas.RequestBroker.Models;

namespace Roaa.Rosas.Infrastructure.ExternalSystemsAPI
{
    public class WebhookAPI : IWebhookAPI
    {
        private readonly ILogger<ExternalSystemAPI> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IRequestBroker _requestBroker;
        private readonly IRosasDbContext _dbContext;
        public WebhookAPI(IRequestBroker requestBroker,
                                    IWebHostEnvironment environment,
                                    IRosasDbContext dbContext,
                                     ILogger<ExternalSystemAPI> logger)
        {
            _requestBroker = requestBroker;
            _environment = environment;
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task CallWebhookEndpointsAsync<TPayload>(WebhookCallingModel<GlobalPayload<TPayload>> model, CancellationToken cancellationToken = default)
        {
            foreach (var WebhookEndpoint in model.WebhookEndpoints)
            {
                var _requestModel = new RequestModel<GlobalPayload<TPayload>>(
                     uri: WebhookEndpoint.Url,
                     data: model.Payload,
                     requestAuthorization: new RequestAuthorizationModel("Bearer", ""),
                     header: ("apiKey", WebhookEndpoint.Secret)
                     );
                var result = await _requestBroker.PostAsync<dynamic, GlobalPayload<TPayload>>(_requestModel, cancellationToken);
            }

        }
    }
}
