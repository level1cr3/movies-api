namespace Movies.Application.Shared.Foundation;

public class Result
{
    protected internal Result(bool isSuccess, List<Error>? errors = null)
    {
        switch (isSuccess)
        {
            case true when errors?.Count > 0:
                throw new InvalidOperationException("Success result cannot contain errors.");
            case false when (errors is null || errors.Count == 0):
                throw new InvalidOperationException("Failure result must contain at least one error.");
        }


        IsSuccess = isSuccess;
        Errors = errors is null ? [] : [..errors];
    }

    public bool IsSuccess { get; }
    public IReadOnlyList<Error> Errors { get; }

    public static Result Success() => new(true);
    public static Result Failure(List<Error> errors) => new(false, errors);
    
    public static Result<TValue> Success<TValue>(TValue value) => new(value,true);
    public static Result<TValue> Failure<TValue>(List<Error> errors) => new(default, false, errors);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value,bool isSuccess, List<Error>? errors = null) : base(isSuccess, errors)
    {
        _value = value;
    }

    public TValue Value => IsSuccess ? _value! : 
        throw new InvalidOperationException("The value of failure result can not be accessed.");
    
}