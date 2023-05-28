using Microsoft.Extensions.Options;
using Roaa.Rosas.Common.Models;

namespace Roaa.Rosas.Common.ApiConfiguration
{
    public class ApiConfigurationService<TOption> : IApiConfigurationService<TOption> where TOption : BaseOptions
    {
        public TOption Options { get; }

        public ApiConfigurationService(IOptions<TOption> options)
        {
            Options = options.Value;
        }
    }
}
