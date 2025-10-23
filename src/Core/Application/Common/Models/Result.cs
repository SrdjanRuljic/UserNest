namespace Application.Common.Models
{
    public class Result
    {
        public string[] Errors { get; set; }

        public bool Succeeded { get; set; }

        internal Result(bool succeeded, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Errors = [.. errors];
        }

        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result(false, errors);
        }

        public static Result Success()
        {
            return new Result(true, []);
        }
    }
}