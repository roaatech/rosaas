using FluentValidation;
using MediatR;
using Roaa.Rosas.Common.Models.Results;

namespace CleanArchitecture.Application.Common.Behaviours;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result, new()
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
            .ToList();


            if (failures.Any())
            {
                if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var messages = (Result.New().WithErrors(failures)).Messages;
                    // Handle failure when TResponse is Result<T>
                    // Extract the failure message from the validationResult and create a new Result<T> instance with the failure message
                    var failureMessage = failures;
                    var failureResponseType = typeof(TResponse).GetGenericArguments()[0];
                    var failureResultType = typeof(Result<>).MakeGenericType(failureResponseType);
                    var failureResult = Activator.CreateInstance(failureResultType, messages);
                    return (TResponse)failureResult;

                }
                else if (typeof(TResponse) == typeof(Result))
                {
                    // Handle failure when TResponse is Result
                    return (TResponse)await Task.FromResult(Result.New().WithErrors(failures));
                }
            }

        }
        return await next();
    }


}
