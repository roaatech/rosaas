using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Settings;
using System.ComponentModel;
using System.Globalization;

namespace Roaa.Rosas.Application.Services.Management.Settings
{
    public class SettingService : ISettingService
    {
        #region Props 
        private readonly ILogger<SettingService> _logger;
        private readonly IRosasDbContext _dbContext;
        #endregion


        #region Corts
        public SettingService(ILogger<SettingService> logger,
                              IRosasDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        #endregion


        #region Services
        public async Task<Result<T>> LoadSettingAsync<T>(CancellationToken cancellationToken = default) where T : ISettings, new()
        {
            return Result<T>.Successful((T)(await LoadSettingAsync(typeof(T), cancellationToken)).Data);
        }

        public async Task<Result<ISettings>> LoadSettingAsync(Type type, CancellationToken cancellationToken = default)
        {
            var settings = Activator.CreateInstance(type);

            var settingsEntities = await GetSettingsListBySharedKeyAsync(type.Name + ".", cancellationToken);

            foreach (var prop in type.GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = type.Name + "." + prop.Name;

                var setting = GetSettingValueByKey<string>(key, settingsEntities);
                if (setting == null)
                    continue;


                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting))
                    continue;

                var value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return Result<ISettings>.Successful(settings as ISettings);
        }

        public async Task<Result<bool>> AnySettingAsync<T>(CancellationToken cancellationToken = default) where T : ISettings, new()
        {
            Type type = typeof(T);

            var settings = Activator.CreateInstance(type);

            string sharedKey = type.Name + ".";

            return Result<bool>.Successful(await _dbContext.Settings
                                                              .AsNoTracking()
                                                              .Where(x => x.Key.StartsWith(sharedKey))
                                                              .AnyAsync(cancellationToken));
        }
        public async Task<Result> SaveSettingAsync<T>(T settings, CancellationToken cancellationToken = default) where T : ISettings, new()
        {
            var settingsInstance = Activator.CreateInstance(typeof(T));

            var settingsEntities = await GetSettingsListBySharedKeyAsync(typeof(T).Name + ".", cancellationToken);

            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                var value = prop.GetValue(settings, null);
                if (value != null)
                    SetSetting(prop.PropertyType, key, value, settingsEntities);
                else
                    SetSetting(key, string.Empty, settingsEntities);
            }

            await _dbContext.SaveChangesAsync();

            return Result.Successful();
        }

        #endregion


        #region Utilities
        private void SetSetting<T>(string key, T value, List<Setting> allSettings)
        {
            SetSetting(typeof(T), key, value, allSettings);
        }

        private void SetSetting(Type type, string key, object value, List<Setting> allSettings)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            key = key.Trim().ToLowerInvariant();
            var valueStr = TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);


            var setting = allSettings.Where(x => x.Key.Equals(key)).SingleOrDefault();
            if (setting != null)
            {
                //update 
                setting.Value = valueStr;
                _dbContext.Entry(setting).State = EntityState.Modified;
            }
            else
            {
                //insert
                _dbContext.Settings.Add(BuildSettingEntity(key, valueStr));
            }
        }

        private T GetSettingValueByKey<T>(string key, List<Setting> settings, T defaultValue = default)
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            key = key.Trim().ToLowerInvariant();
            if (!settings.Where(x => x.Key.Equals(key)).Any())
                return defaultValue;

            var setting = settings.Where(x => x.Key.Equals(key)).SingleOrDefault();


            return setting != null ? To<T>(setting.Value) : defaultValue;
        }

        private async Task<List<Setting>> GetSettingsListBySharedKeyAsync(string sharedKey, CancellationToken cancellationToken = default)
        {
            var settings = await _dbContext.Settings
                                            .AsNoTracking()
                                            .Where(x => x.Key.StartsWith(sharedKey))
                                            .ToListAsync(cancellationToken);
            return settings;
        }


        private Setting BuildSettingEntity(string key, string valueStr)
        {
            return new Setting
            {
                Id = Guid.NewGuid(),
                Key = key,
                Value = valueStr,
            };
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        private T To<T>(object value)
        {
            //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            return (T)To(value, typeof(T));
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        private object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        private object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value == null)
                return null;

            var sourceType = value.GetType();

            var destinationConverter = TypeDescriptor.GetConverter(destinationType);
            if (destinationConverter.CanConvertFrom(value.GetType()))
                return destinationConverter.ConvertFrom(null, culture, value);

            var sourceConverter = TypeDescriptor.GetConverter(sourceType);
            if (sourceConverter.CanConvertTo(destinationType))
                return sourceConverter.ConvertTo(null, culture, value, destinationType);

            if (destinationType.IsEnum && value is int)
                return Enum.ToObject(destinationType, (int)value);

            if (!destinationType.IsInstanceOfType(value))
                return Convert.ChangeType(value, destinationType, culture);

            return value;
        }

        #endregion

    }
}
