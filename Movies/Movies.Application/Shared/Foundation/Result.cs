namespace Movies.Application.Shared.Foundation;

public class Result
{
    protected internal Result(bool isSuccess, List<AppError>? appErrors = null)
    {
        switch (isSuccess)
        {
            case true when appErrors?.Count > 0:
                throw new InvalidOperationException("Success result cannot contain errors.");
            case false when (appErrors is null || appErrors.Count == 0):
                throw new InvalidOperationException("Failure result must contain at least one error.");
        }


        IsSuccess = isSuccess;
        AppErrors = appErrors is null ? [] : [..appErrors];
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<AppError> AppErrors { get; }

    public static Result Success() => new(true);
    public static Result Failure(List<AppError> appErrors) => new(false, appErrors);
    
    public static Result<TValue> Success<TValue>(TValue value) => new(value,true);
    public static Result<TValue> Failure<TValue>(List<AppError> appErrors) => new(default, false, appErrors);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value,bool isSuccess, List<AppError>? appErrors = null) : base(isSuccess, appErrors)
    {
        _value = value;
    }

    public TValue Value => IsSuccess ? _value! : 
        throw new InvalidOperationException("The value of failure result can not be accessed.");
    
}