namespace PortGuard.Application.Common.Results
{
    public class Result
    {
        public bool IsSuccess => string.IsNullOrWhiteSpace(Error);
        public bool IsFailed => !IsSuccess;
        public string Error { get; private set; } = string.Empty;

        private Result()
        {
        }

        private Result(string error)
            => Error = error;

        public static Result Success()
            => new();

        public static Result Fail(string error)
            => new(error);
    }
}
