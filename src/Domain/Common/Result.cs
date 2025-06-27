using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Domain.Common;



public class Result<T> : Result
{
    public T Data { get; set; }
    public void SetData(T value)
    {
        Data = value;
    }

    //private Result(T value, bool isSuccess, string error)
    //    : base(isSuccess, error)
    //{
    //    Data = value;
    //}

    //  public static Result<T> Success(T value) => new(value, true, string.Empty);


    //  public static new Result<T> Failure(string error) => new(default!, false, error);
}

public class Result
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public IDictionary<string, string[]> Errors { get; set; }
    public bool IsFailure => !IsSuccess;

    //protected Result(bool isSuccess, string error)
    //{
    //    if (isSuccess && error != string.Empty)
    //        throw new InvalidOperationException();
    //    if (!isSuccess && error == string.Empty)
    //        throw new InvalidOperationException();

    //    IsSuccess = isSuccess;
    //    Error = error;
    //}

    // public static Result Success() => new(true, string.Empty);

    public static Result Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static Result<T> Success<T>(T data)
    {
        return new Result<T>()
        {
            IsSuccess = true,
            Data = data
        };
    }

    public static Result<T> Success<T>(T data, string message)
    {
        return new Result<T>()
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };
    }


    public static Result<PaginatedList<T>> Success<T>(PaginatedList<T> data)
    {
        return new Result<PaginatedList<T>>()
        {
            IsSuccess = true,
            Data = data
        };
    }

    public static Result<PaginatedList<T>> Success<T>(PaginatedList<T> data, string message)
    {
        return new Result<PaginatedList<T>>()
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };
    }


    public static Result ValidationError<T>(IEnumerable<ValidationError> validationErrors, string message)
    {
        IDictionary<string, string[]> errors = new Dictionary<string, string[]>();
        var failureGroups = validationErrors
        .GroupBy(e => e.Name, e => e.Reason);

        foreach (var failureGroup in failureGroups)
        {
            var propertyName = failureGroup.Key;
            var propertyFailures = failureGroup.ToArray();
            errors.Add(propertyName, propertyFailures);
        }

        return new Result<T>()
        {
            IsSuccess = true,
            Message = message,
            Errors = errors
        };
    }

    public static Result ValidationError<T>(IDictionary<string, string[]> validationErrors, string message)
    {
        return new Result<T>()
        {
            IsSuccess = false,
            Message = message,
            Errors = validationErrors
        };
    }
    public static Result ValidationError(IDictionary<string, string[]> validationErrors, string message)
    {
        return new Result()
        {
            IsSuccess = false,
            Message = message,
            Errors = validationErrors
        };
    }

    public static Result Failure(string message)
    {
        return new Result()
        {
            IsSuccess = false,
            Message = message
        };
    }

}




