using Domain.Common;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Common.Behaviours;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
 where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var validationTasks = _validators
       .Select(v => v.ValidateAsync(context, cancellationToken)) // Call ValidateAsync
       .ToList();

        var validationResults = await Task.WhenAll(validationTasks);

        var failures = validationResults
       .SelectMany(result => result.Errors)
       .Where(f => f != null)
       .ToList();

        if (failures.Any())
        {
            return (TResponse)(object)CreateValidationErrorResponse(failures);
        }
        return await next();
    }

    private IResult CreateValidationErrorResponse(IEnumerable<ValidationFailure> failures)
    {
        var errorResponse = new
        {
            Errors = failures
                .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
                .ToDictionary(g => g.Key, g => g.ToArray()),
            Message = "Validation error"
        };
        var response = Result.ValidationError(errorResponse.Errors, errorResponse.Message);
        return Results.Json(response, statusCode: StatusCodes.Status400BadRequest);
       // return Results.ValidationProblem(errorResponse.Errors, errorResponse.Message);
    }
}

public class ValidationErrorResponse
{
    public bool Succeeded { get; set; }
    public string Message { get; set; }
    public IDictionary<string, string[]> Errors { get; set; }

    public ValidationErrorResponse()
    {
        Succeeded = false;
        Message = "Validation error";
        Errors = new Dictionary<string, string[]>();
    }
}


