namespace FluxoCaixa.BuildingBlocks.Application.Results;

public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(T value)
        : base(true, Error.None)
    {
        Value = value;
    }

    protected Result(Error error)
        : base(false, error)
    {
        Value = default;
    }

    public static Result<T> Success(T value) => new(value);

    public static new Result<T> Failure(Error error) => new(error);
}