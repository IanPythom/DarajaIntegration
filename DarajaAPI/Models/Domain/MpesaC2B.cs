using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DarajaAPI.Models.Domain
{
    public class MpesaC2B
    {
        [Key]
        public int Id { get; set; }

        [JsonProperty("TransactionType")]
        public string TransactionType { get; set; }

        [JsonProperty("TransID")]
        public string TransID { get; set; }

        [JsonProperty("TransTime")]
        public string TransTime { get; set; }

        [JsonProperty("TransAmount")]
        public string TransAmount { get; set; }

        [JsonProperty("BusinessShortCode")]
        public string BusinessShortCode { get; set; }

        [JsonProperty("BillRefNumber")]
        public string BillRefNumber { get; set; }

        [JsonProperty("InvoiceNumber")]
        public string InvoiceNumber { get; set; }

        [JsonProperty("OrgAccountBalance")]
        public string OrgAccountBalance { get; set; }

        [JsonProperty("ThirdPartyTransID")]
        public string ThirdPartyTransID { get; set; }

        [JsonProperty("MSISDN")]
        public string MSISDN { get; set; }

        // Optional fields (may not be present in all callbacks)
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string VerificationResult { get; set; }
        public int RetryCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
