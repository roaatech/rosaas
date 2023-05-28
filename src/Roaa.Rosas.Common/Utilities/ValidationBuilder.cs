using FluentValidation.Results;
using Roaa.Rosas.Common.Localization;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Common.Utilities
{
    public class ValidationBuilder
    {
        #region Props 
        private readonly List<Func<Task<Result>>> _commands = new();
        private readonly LanguageEnum _locale;
        private Result _result;
        #endregion


        #region Corts
        public ValidationBuilder(LanguageEnum locale)
        {
            _locale = locale;
        }
        #endregion


        #region Main Validator 
        public async Task<Result> ValidateAsync()
        {

            foreach (var command in _commands)
            {
                var result = await command();
                if (!result.Success)
                {
                    _commands.Clear();
                    _result = result;
                    return result;
                }
            }

            _commands.Clear();
            if (_result is not null)
            {
                return _result;
            }
            return Result.Successful();
        }
        #endregion


        #region Commands
        public void AddCommand<T>(Func<Task<T>> func, Enum error) where T : class
        {
            _commands.Add(async () => await ValidatAsync(func, error));
        }
        public void AddCommand(Func<Task<Result>> func)
        {
            _commands.Add(async () => await ValidatAsync(func));
        }
        public void AddCommand(Func<ValidationResult> func)
        {
            _commands.Add(async () => await ValidatAsync(func));
        }
        public void AddCommand(Func<Task<bool>> func, Enum error)
        {
            _commands.Add(async () => await ValidatAsync(func, error));
        }
        public void AddCommand(Func<bool> func, Enum error)
        {
            _commands.Add(() => ValidatAsync(func, error));
        }
        #endregion



        #region Validators 
        private async Task<Result> ValidatAsync(Func<Task<Result>> func)
        {
            var result = await func();
            if (!result.Success)
            {
                return Result.Fail(result.Messages);
            }

            return Result.Successful();
        }

        private async Task<Result> ValidatAsync(Func<ValidationResult> func)
        {
            var validationResult = func();
            if (!validationResult.IsValid)
            {
                return Result.New().WithErrors(validationResult.Errors);
            }

            return Result.Successful();
        }

        private async Task<Result> ValidatAsync(Func<Task<bool>> func, Enum error)
        {
            if (!await func())
            {
                return Result.Fail(error, _locale);
            }

            return Result.Successful();
        }

        private async Task<Result> ValidatAsync(Func<bool> func, Enum error)
        {
            if (!func())
            {
                return Result.Fail(error, _locale);
            }

            return Result.Successful();
        }

        private async Task<Result> ValidatAsync<T>(Func<Task<T>> func, Enum error)
        {
            T obj = await func();
            if (obj is null)
            {
                return Result.Fail(error, _locale);
            }

            return Result.Successful();
        }

        #endregion

    }
}
