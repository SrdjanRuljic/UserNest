namespace Application.Users.Queries.GetAll
{
    public static class GetAllUsersQueryValidator
    {
        public static bool IsValid(this GetAllUsersQuery model, out string validationMessage)
        {
            validationMessage = string.Empty;
            bool isValid = true;

            if (model == null)
            {
                validationMessage = Resources.Translation.ModelCanNotBeNull;
                isValid = false;
            }

            if (model!.PageNumber <= 0)
            {
                validationMessage += Resources.Translation.PageNumberMustBeGreaterThanZero;
                isValid = false;
            }

            if (model.PageSize <= 0)
            {
                validationMessage += Resources.Translation.PageSizeMustBeGreaterThanZero;
                isValid = false;
            }

            if (model.PageSize > 100)
            {
                validationMessage += Resources.Translation.PageSizeCannotExceedHundred;
                isValid = false;
            }

            return isValid;
        }
    }
}