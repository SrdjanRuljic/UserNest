using System.Text.RegularExpressions;

namespace Application.Users.Commands.Insert
{
    public static class InsertUserCommandValidator
    {
        #region Constants

        private const int FIRST_NAME_MAX_LENGTH = 100;
        private const int LAST_NAME_MAX_LENGTH = 100;
        private const int USERNAME_MAX_LENGTH = 256;
        private const int EMAIL_MAX_LENGTH = 256;
        private const int PASSWORD_MIN_LENGTH = 6;
        private const int PASSWORD_MAX_LENGTH = 100;
        private const int PHONE_NUMBER_MAX_LENGTH = 20;

        #endregion Constants

        #region Public Methods

        public static bool IsValid(this InsertUserCommand model, out string validationMessage)
        {
            validationMessage = string.Empty;

            if (model == null)
            {
                validationMessage = Resources.Translation.ModelCanNotBeNull;
                return false;
            }

            bool isValid = true;

            isValid &= ValidatePersonalInfo(model, ref validationMessage);
            isValid &= ValidateCredentials(model, ref validationMessage);
            isValid &= ValidateEmail(model, ref validationMessage);
            isValid &= ValidatePhoneNumber(model, ref validationMessage);
            isValid &= ValidateRole(model, ref validationMessage);

            return isValid;
        }

        #endregion Public Methods

        #region Private Validation Methods

        private static bool ValidatePersonalInfo(InsertUserCommand model, ref string validationMessage)
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(model.FirstName))
            {
                validationMessage += Resources.Translation.FirstNameRequired;
                isValid = false;
            }
            else if (model.FirstName.Length > FIRST_NAME_MAX_LENGTH)
            {
                validationMessage += string.Format(Resources.Translation.FirstNameTooLong, FIRST_NAME_MAX_LENGTH);
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(model.LastName))
            {
                validationMessage += Resources.Translation.LastNameRequired;
                isValid = false;
            }
            else if (model.LastName.Length > LAST_NAME_MAX_LENGTH)
            {
                validationMessage += string.Format(Resources.Translation.LastNameTooLong, LAST_NAME_MAX_LENGTH);
                isValid = false;
            }

            if (model.LanguageId.HasValue && model.LanguageId.Value <= 0)
            {
                validationMessage += Resources.Translation.LanguageIdValid;
                isValid = false;
            }

            return isValid;
        }

        private static bool ValidateCredentials(InsertUserCommand model, ref string validationMessage)
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(model.UserName))
            {
                validationMessage += Resources.Translation.UsernameRequired;
                isValid = false;
            }
            else
            {
                if (model.UserName.Length > USERNAME_MAX_LENGTH)
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

        private static bool ValidateEmail(InsertUserCommand model, ref string validationMessage)
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                validationMessage += Resources.Translation.EmailRequired;
                isValid = false;
            }
            else
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

        private static bool ValidatePhoneNumber(InsertUserCommand model, ref string validationMessage)
        {
            bool isValid = true;

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                if (model.PhoneNumber.Length > PHONE_NUMBER_MAX_LENGTH)
                {
                    validationMessage += string.Format(Resources.Translation.PhoneNumberTooLong, PHONE_NUMBER_MAX_LENGTH);
                    isValid = false;
                }
                else if (!IsValidPhoneNumberFormat(model.PhoneNumber))
                {
                    validationMessage += string.Format(Resources.Translation.InvalidPhoneNumber, model.PhoneNumber);
                    isValid = false;
                }
            }

            return isValid;
        }

        private static bool ValidateRole(InsertUserCommand model, ref string validationMessage)
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(model.Role))
            {
                validationMessage += Resources.Translation.RoleRequired;
                isValid = false;
            }
            else if (!IsValidRole(model.Role))
            {
                validationMessage += string.Format(Resources.Translation.InvalidRole, model.Role);
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

        private static bool IsValidPhoneNumberFormat(string phoneNumber)
        {
            // Allow international format with +, digits, spaces, hyphens, parentheses
            string pattern = @"^[\+]?[1-9][\d]{0,15}$";
            return Regex.IsMatch(phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""), pattern);
        }

        private static bool IsValidRole(string role)
        {
            return Enum.TryParse<Domain.Enums.Roles>(role, true, out _);
        }

        #endregion Helper Methods
    }
}