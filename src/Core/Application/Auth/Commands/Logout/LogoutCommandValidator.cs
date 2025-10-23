namespace Application.Auth.Commands.Logout
{
    public static class LogoutCommandValidator
    {
        public static bool IsValid(this LogoutCommand model, out string validationMessage)
        {
            validationMessage = string.Empty;
            bool isValid = true;

            if (model == null)
            {
                validationMessage = Resources.Translation.ModelCanNotBeNull;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(model!.RefreshToken))
            {
                validationMessage += Resources.Translation.RefreshTokenRequired;
                isValid = false;
            }

            return isValid;
        }
    }
}