using System.ComponentModel.DataAnnotations;

namespace DarajaAPI.Models.Dto
{
    public class LipaNaTransactionDto
    {
        public string AccountNumberPrefix { get; } = "2173219";
        public DateTime TransactionTimestamp { get; set; } = DateTime.Now;
        public string TransactionType { get; set; } = "BuyGoods";
        public string BusinessNumber { get; set; } = "22111";
        public string AccountNumber { get; set; }
        public string ContactNumber { get; set; } = "0703095445";
        public string BusinessName { get; set; } = "IAN MWENDA GATUMU";

        [StringLength(16, MinimumLength = 16)]
        public string FormattedAccountNumber => $"{AccountNumberPrefix}{TransactionTimestamp:yyyyMMddHHmmss}";
    }
}
