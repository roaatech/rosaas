using Roaa.Rosas.Common.EFCore;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Services.Auth.Builders
{
    public class CommandBuilder<T, TDbContext> where TDbContext : IDbContext
    {
        #region Props  
        private readonly List<Func<Task<Result>>> _commands;
        private readonly TDbContext _dbContext;

        private Func<Task<T>> _detailsFunc;
        #endregion


        #region Corts
        public CommandBuilder(TDbContext dbContext)
        {
            _commands = new();
            _dbContext = dbContext;
        }
        #endregion


        #region main method 
        public async Task<Result> InvokeAsync()
        {
            using (var scope = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var command in _commands)
                    {
                        var _result = await command();
                        if (!_result.Success)
                        {
                            scope.Rollback();
                            return Result.Fail(_result.Messages);
                        }
                    }
                    scope.Commit();
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    throw;
                }
            }

            _commands.Clear();

            return Result.Successful();
        }
        #endregion


        #region methods   

        #endregion


        #region commands
        public void AddCommand(Func<Task<Result>> func)
        {
            _commands.Add(func);
        }
        public void AddCommand<T>(Func<Task<T>> func)
        {
            _commands.Add(async () => await ValidatAsync(func));
        }
        public void AddCommand(Func<Task> func)
        {
            _commands.Add(async () => await ValidatAsync(func));
        }
        public void AddCommand(Action func)
        {
            _commands.Add(() => Validat(func));
        }




        private async Task<Result> ValidatAsync(Func<Task> func)
        {
            await func();

            return Result.Successful();
        }
        private async Task<Result> Validat(Action func)
        {
            func();
            return Result.Successful();
        }

        #endregion

    }

}
