using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Management.Settings
{
    public interface ISettingService
    {
        Task<Result<T>> LoadSettingAsync<T>(CancellationToken cancellationToken = default) where T : ISettings, new();

        Task<Result<ISettings>> LoadSettingAsync(Type type, CancellationToken cancellationToken = default);

        Task<Result<bool>> AnySettingAsync<T>(CancellationToken cancellationToken = default) where T : ISettings, new();

        Task<Result> SaveSettingAsync<T>(T settings, CancellationToken cancellationToken = default) where T : ISettings, new();


    }
}
