namespace Application.Exceptions
{
    public static class ErrorMessages
    {
        public static string Forbidden => Resources.Translation.Forbidden;
        public static string Unauthorized => Resources.Translation.Unauthorised;

        public static string CreateEntityWasNotFoundMessage(string name, object key)
            => string.Format(Resources.Translation.EntityWasNotFound, name, key);
    }
}