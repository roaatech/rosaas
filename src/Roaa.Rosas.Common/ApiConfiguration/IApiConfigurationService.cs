using Roaa.Rosas.Common.Models;

namespace Roaa.Rosas.Common.ApiConfiguration
{
    public interface IApiConfigurationService<TOption> where TOption : BaseOptions
    {
        public TOption Options { get; }
    }
}
