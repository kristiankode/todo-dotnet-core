using System.Text.RegularExpressions;

namespace ToDoApi.Validation
{
    public class NameValidator
    {
        private const int NameLimit = 10;
        public const string NameValidatePattern = @"^[a-zA-Z0-9\ ]+$";

        public static bool IsValid(string name)
        {
            //return name == "Valid name";
            return !string.IsNullOrWhiteSpace(name)
                   && name.Length <= NameLimit
                   && new Regex(NameValidatePattern).IsMatch(name);
        }
    }
}