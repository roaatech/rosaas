using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Settings;

namespace Roaa.Rosas.Application.Services.Management.Settings
{
    public interface ISettingService
    {
        Task<Result<List<Setting>>> GetSettingsListAsync(Type type, CancellationToken cancellationToken = default);

        Task<Result<T>> LoadSettingAsync<T>(CancellationToken cancellationToken = default) where T : ISettings, new();

        Task<Result<ISettings>> LoadSettingAsync(Type type, CancellationToken cancellationToken = default);

        Task<Result<bool>> AnySettingAsync<T>(CancellationToken cancellationToken = default) where T : ISettings, new();

        Task<Result> SaveSettingAsync<T>(T settings, CancellationToken cancellationToken = default) where T : ISettings, new();


    }
}
