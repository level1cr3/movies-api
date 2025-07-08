﻿namespace Movies.Application.Shared.Foundation;

public class Result
{
    protected internal Result(bool isSuccess, Error error)
    {
        if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
        {
            throw new InvalidOperationException();
        }
        
        IsSuccess = isSuccess;
        Error = error;
    }
    
    public bool IsSuccess { get; }
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);
    
    public static Result<TValue> Success<TValue>(TValue value) => new(value,true, Error.None);

    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value,bool isSuccess, Error error) : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("The value of failure result can not be accessed.");
}