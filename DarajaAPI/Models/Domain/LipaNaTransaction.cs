using System.ComponentModel.DataAnnotations;

namespace DarajaAPI.Models.Domain
{
    public class LipaNaTransaction
    {
        public string AccountNumberPrefix { get; } = "2173219";
        public DateTime TransactionTimestamp { get; set; } = DateTime.Now;
        public string TransactionType { get; set; } = "BuyGoods";
        public string BusinessNumber { get; set; } = "2211";
        public string AccountNumber { get; set; }
        public string ContactNumber { get; set; } = "0703095445";
        public string BusinessName { get; set; } = "IAN MWENDA GATUMU";

        [StringLength(16, MinimumLength = 16)]
        public string FormattedAccountNumber => $"{AccountNumberPrefix}{TransactionTimestamp:yyyyMMddHHmmss}";
    }
}
