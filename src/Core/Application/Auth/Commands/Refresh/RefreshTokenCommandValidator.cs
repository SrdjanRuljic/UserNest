namespace Application.Auth.Commands.Refresh
{
    public static class RefreshTokenCommandValidator
    {
        public static bool IsValid(this RefreshTokenCommand model, out string validationMessage)
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