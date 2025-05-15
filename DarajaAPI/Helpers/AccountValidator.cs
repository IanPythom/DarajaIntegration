using System.Globalization;

namespace DarajaAPI.Helpers
{
    public class AccountValidator
    {
        public static (bool IsValid, string Error) ValidateAccountNumber(string accountNumber)
        {
            const string prefix = "2173219";
            const int requiredLength = 21; // 7 (prefix) + 14 (timestamp)

            if (!accountNumber.StartsWith(prefix))
                return (false, $"Account must start with {prefix}");

            if (accountNumber.Length != requiredLength)
                return (false, $"Invalid length. Expected {requiredLength} characters");

            if (!DateTime.TryParseExact(accountNumber[7..],
                "yyyyMMddHHmmss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _))
                return (false, "Invalid timestamp format");

            return (true, null);
        }
    }
}
