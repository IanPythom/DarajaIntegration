using System.ComponentModel.DataAnnotations;

namespace DarajaAPI.Models.Domain
{
    public class DarajaSetting
    {
        [Key]
        public int Id { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string ShortCode { get; set; }
        public string ResponseType { get; set; }
        public string ConfirmationURL { get; set; }
        public string ValidationURL { get; set; }
        public string PassKey { get; set; }
        public string C2BSimulateUrl { get; set; } = "mpesa/c2b/v1/simulate";
        public string TransactionStatusEndpoint { get; set; } = "mpesa/transactionstatus/v1/query";
        public string BaseUrl { get; set; }
        public string InitiatorName { get; set; }
        public string InitiatorPassword { get; set; }
        public string RegisterUrlEndpoint { get; set; } // Add this line
    }
}
