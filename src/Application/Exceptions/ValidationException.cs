using FluentValidation.Results;
namespace Application.Exceptions;
public class ValidationException : Exception
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = "One or more validation failures have occurred.";
    public IDictionary<string, string[]> Errors { get; }
    public ValidationException() : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Success = false;
        Message = "Validation error";

        var failureGroups = failures
           .GroupBy(e => e.PropertyName, e => e.ErrorMessage);

        foreach (var failureGroup in failureGroups)
        {
            var propertyName = failureGroup.Key;
            var propertyFailures = failureGroup.ToArray();

            Errors.Add(propertyName, propertyFailures);
        }
    }

}