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
        public string MpesaBaseUrl { get; set; }
        public string RegisterUrlEndpoint { get; set; } // Add this line
    }
}
