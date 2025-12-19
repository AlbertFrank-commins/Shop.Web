namespace Shop.BuildingBlocks.Results
{
    public class Result<T>
    {
        public bool Success { get; private set; }
        public string? Error { get; private set; }
        public T? Value { get; private set; }

        private Result() { }

        public static Result<T> Ok(T value)
            => new Result<T> { Success = true, Value = value };

        public static Result<T> Fail(string error)
            => new Result<T> { Success = false, Error = error };
    }
}
