using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Helper
{
    //public static class ValidationHelper
    //{
    //    public static IResult ProblemFromFailures(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
    //    {
    //        var errors = failures
    //            .GroupBy(f => f.PropertyName)
    //            .ToDictionary(
    //                g => g.Key,
    //                g => g.Select(x => x.ErrorMessage).ToArray()
    //            );

    //        var Result = new
    //        {
    //            success = false,
    //            message = "Validation error",
    //            errors = errors
    //        };

    //        return Results.Json(response, statusCode: StatusCodes.Status400BadRequest);
    //    }
    //}

}
