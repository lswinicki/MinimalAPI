using FluentValidation;
using MediatR;

namespace API.Helpers.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);
        
        var context = new ValidationContext<TRequest>(request);
        
        var validationFailures = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        
        var errors = validationFailures
            .Where(v => v.Errors.Any())
            .SelectMany(v => v.Errors)
            .ToList();
        
        if(errors.Any())
            throw new ValidationException(errors);
        
        var response = await next(cancellationToken);

        return response;
    }
}