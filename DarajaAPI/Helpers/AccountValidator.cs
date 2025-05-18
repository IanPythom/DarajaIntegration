using System.Globalization;

namespace DarajaAPI.Helpers
{
    public class AccountValidator
    {
        public static (bool IsValid, string Error) ValidateAccountNumber(string accountNumber)
        {
            const string prefix = "174379"; // Changed from 2173219
            const int requiredLength = 20;  // 6 (prefix) + 14 (timestamp) e.g 17437920231015124530" (20 characters)

            if (!accountNumber.StartsWith(prefix))
                return (false, $"Account must start with {prefix}");

            if (accountNumber.Length != requiredLength)
                return (false, $"Invalid length. Expected {requiredLength} characters");

            // Validate timestamp (now starts at index 6)
            if (!DateTime.TryParseExact(accountNumber[6..], // Changed from [7..]
                "yyyyMMddHHmmss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _))
                return (false, "Invalid timestamp format");

            return (true, null);
        }
    }
}
