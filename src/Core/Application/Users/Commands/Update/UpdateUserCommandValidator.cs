using System.Text.RegularExpressions;

namespace Application.Users.Commands.Update
{
    public static class UpdateUserCommandValidator
    {
        #region Constants

        private const int FIRST_NAME_MAX_LENGTH = 100;
        private const int LAST_NAME_MAX_LENGTH = 100;
        private const int USERNAME_MAX_LENGTH = 256;
        private const int EMAIL_MAX_LENGTH = 256;
        private const int PASSWORD_MIN_LENGTH = 6;
        private const int PASSWORD_MAX_LENGTH = 100;
        private const int PHONE_NUMBER_MAX_LENGTH = 20;
        private const int USER_ID_MAX_LENGTH = 50;

        #endregion Constants

        #region Public Methods

        public static bool IsValid(this UpdateUserCommand model, out string validationMessage)
        {
            validationMessage = string.Empty;

            if (model == null)
            {
                validationMessage = Resources.Translation.ModelCanNotBeNull;
                return false;
            }

            bool isValid = true;

            isValid &= ValidateUserId(model, ref validationMessage);
            isValid &= ValidatePersonalInfo(model, ref validationMessage);
            isValid &= ValidateCredentials(model, ref validationMessage);
            isValid &= ValidateEmail(model, ref validationMessage);
            isValid &= ValidateLanguageId(model, ref validationMessage);

            return isValid;
        }

        #endregion Public Methods

        #region Private Validation Methods

        private static bool ValidateUserId(UpdateUserCommand model, ref string validationMessage)
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(model.Id))
            {
                validationMessage += Resources.Translation.UserIdRequired;
                isValid = false;
            }
            else if (model.Id.Length > USER_ID_MAX_LENGTH)
            {
                validationMessage += string.Format(Resources.Translation.UserIdTooLong, USER_ID_MAX_LENGTH);
                isValid = false;
            }

            return isValid;
        }

        private static bool ValidatePersonalInfo(UpdateUserCommand model, ref string validationMessage)
        {
            bool isValid = true;

            if (!string.IsNullOrWhiteSpace(model.FirstName))
            {
                if (model.FirstName.Length > FIRST_NAME_MAX_LENGTH)
                {
                    validationMessage += string.Format(Resources.Translation.FirstNameTooLong, FIRST_NAME_MAX_LENGTH);
                    isValid = false;
                }
            }

            if (!string.IsNullOrWhiteSpace(model.LastName))
            {
                if (model.LastName.Length > LAST_NAME_MAX_LENGTH)
                {
                    validationMessage += string.Format(Resources.Translation.LastNameTooLong, LAST_NAME_MAX_LENGTH);
                    isValid = false;
                }
            }

            return isValid;
        }

        private static bool ValidateCredentials(UpdateUserCommand model, ref string validationMessage)
        {
            bool isValid = true;

            if (!string.IsNullOrWhiteSpace(model.UserName))
            {
                if (model.UserName.Length < 3)
                {
                    validationMessage += Resources.Translation.UserNameTooShort;
                    isValid = false;
                }
                else if (model.UserName.Length > USERNAME_MAX_LENGTH)
                {
                    validationMessage += string.Format(Resources.Translation.UserNameTooLong, USERNAME_MAX_LENGTH);
                    isValid = false;
                }
                else if (!IsValidUserNameFormat(model.UserName))
                {
                    validationMessage += string.Format(Resources.Translation.InvalidUserName, model.UserName);
                    isValid = false;
                }
            }

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (model.Password.Length < PASSWORD_MIN_LENGTH)
                {
                    validationMessage += string.Format(Resources.Translation.PasswordTooShort, PASSWORD_MIN_LENGTH);
                    isValid = false;
                }
                else if (model.Password.Length > PASSWORD_MAX_LENGTH)
                {
                    validationMessage += string.Format(Resources.Translation.PasswordTooLong, PASSWORD_MAX_LENGTH);
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

        private static bool ValidateEmail(UpdateUserCommand model, ref string validationMessage)
        {
            bool isValid = true;

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                if (model.Email.Length > EMAIL_MAX_LENGTH)
                {
                    validationMessage += string.Format(Resources.Translation.EmailTooLong, EMAIL_MAX_LENGTH);
                    isValid = false;
                }
                else if (!model.Email.IsValidEmail())
                {
                    validationMessage += string.Format(Resources.Translation.InvalidEmail, model.Email);
                    isValid = false;
                }
            }

            return isValid;
        }

        private static bool ValidateLanguageId(UpdateUserCommand model, ref string validationMessage)
        {
            bool isValid = true;

            if (model.LanguageId.HasValue && model.LanguageId.Value <= 0)
            {
                validationMessage += Resources.Translation.LanguageIdValid;
                isValid = false;
            }

            return isValid;
        }

        #endregion Private Validation Methods

        #region Helper Methods

        private static bool IsValidUserNameFormat(string userName)
        {
            return userName.All(x => char.IsLetterOrDigit(x) || x == '-' || x == '.' || x == '_' || x == '@' || x == '+');
        }

        private static bool IsValidEmail(this string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" +
                             @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" +
                             @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            Regex regex = new(pattern, RegexOptions.IgnoreCase);

            return regex.IsMatch(email);
        }

        private static bool IsValidPassword(this string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$";
            return Regex.IsMatch(password, pattern);
        }

        #endregion Helper Methods
    }
}