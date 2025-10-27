using System.Text.RegularExpressions;

namespace Application.Users.Queries.ValidatePassword
{
    public static class ValidatePasswordQueryValidator
    {
        #region Constants

        private const int PASSWORD_MIN_LENGTH = 6;

        #endregion Constants

        #region Public Methods

        public static bool IsValid(this ValidatePasswordQuery model, out string validationMessage)
        {
            validationMessage = string.Empty;

            if (model == null)
            {
                validationMessage = Resources.Translation.ModelCanNotBeNull;
                return false;
            }

            return ValidatePassword(model, ref validationMessage);
        }

        #endregion Public Methods

        #region Private Validation Methods

        private static bool ValidatePassword(ValidatePasswordQuery model, ref string validationMessage)
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                validationMessage += Resources.Translation.PasswordRequired;
                isValid = false;
            }
            else
            {
                if (model.Password.Length < PASSWORD_MIN_LENGTH)
                {
                    validationMessage += string.Format(Resources.Translation.PasswordTooShort, PASSWORD_MIN_LENGTH);
                    isValid = false;
                }
                else if (!model.Password.IsValidPassword())
                {
                    validationMessage += Resources.Translation.PasswordMustContain;
                    isValid = false;
                }
            }

            return isValid;
        }

        #endregion Private Validation Methods

        #region Helper Methods

        private static bool IsValidPassword(this string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$";
            return Regex.IsMatch(password, pattern);
        }

        #endregion Helper Methods
    }
}
