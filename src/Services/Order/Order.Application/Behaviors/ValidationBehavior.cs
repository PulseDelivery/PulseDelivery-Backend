using FluentValidation;
using MediatR;

namespace Order.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // If there is no validator defined for this request, proceed directly to the Handler
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        // Run all validators asynchronously
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Collect all validation errors
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            // If there are validation errors, stop the pipeline and throw a FluentValidation exception.
            // The CustomExceptionMiddleware in the Shared project will catch this exception
            // and automatically convert it into a 400 Bad Request ResponseDto.
            throw new ValidationException(failures);
        }

        // If there are no errors, delegate the request to the actual Handler
        return await next();
    }
}