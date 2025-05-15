namespace DarajaAPI.Models.Dto
{
    public class PaymentResultDto
    {
        public string TransactionCode { get; set; }
        public decimal Amount { get; set; }
        public string ResponseCode { get; set; }
        public string Description { get; set; }
    }
}
