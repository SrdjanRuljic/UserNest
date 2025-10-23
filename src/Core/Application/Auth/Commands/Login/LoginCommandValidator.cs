namespace Application.Auth.Commands.Login
{
    public static class LoginCommandValidator
    {
        public static bool IsValid(this LoginCommand model, out string validationMessage)
        {
            validationMessage = string.Empty;
            bool isValid = true;

            if (model == null)
            {
                validationMessage = Resources.Translation.ModelCanNotBeNull;
                isValid = false;
            }

            if (String.IsNullOrWhiteSpace(model?.Username))
            {
                validationMessage += Resources.Translation.UsernameRequired;
                isValid = false;
            }

            if (String.IsNullOrWhiteSpace(model?.Password))
            {
                validationMessage += Resources.Translation.PasswordRequired;
                isValid = false;
            }

            return isValid;
        }
    }
}