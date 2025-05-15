namespace DarajaAPI.Records
{
    public record TransactionStatusResult(
            bool IsSuccessful,
            string ResultCode,
            string ResultDescription,
            string TransactionStatus
        );
}
