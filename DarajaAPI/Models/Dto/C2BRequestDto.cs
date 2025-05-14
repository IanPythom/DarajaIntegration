using Newtonsoft.Json;

namespace DarajaAPI.Models.Dto
{
    public class C2BRequestDto
    {
        [JsonProperty("Msisdn")] // Must match exactly
        public string PhoneNumber { get; set; }

        [JsonProperty("BillRefNumber")] // Not "AccountNumber"
        public string AccountNumber { get; set; }

        [JsonProperty("Amount")]
        public string Amount { get; set; }
    }
}
