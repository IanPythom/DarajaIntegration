using Newtonsoft.Json;

namespace DarajaAPI.Models.Dto
{
    public class DarajaResponseDto
    {
        /// Safaricom's response code (0 = success)
        [JsonProperty("ResponseCode")]
        public string ResponseCode { get; set; }

        [JsonProperty("ResponseDescription")]
        public string ResponseDescription { get; set; }

        /// Unique identifier for the API request (from Safaricom)
        [JsonProperty("OriginatorConversationID")]
        public string OriginatorConversationID { get; set; }

        /// Additional conversation identifier
        [JsonProperty("ConversationID")]
        public string ConversationID { get; set; }

        /// Present in error responses
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        /// Error description (if request failed)
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        // For STK/Payment Requests
        [JsonProperty("MerchantRequestID")]
        public string MerchantRequestID { get; set; }

        [JsonProperty("CheckoutRequestID")]
        public string CheckoutRequestID { get; set; }

        /// Optional: Raw JSON for debugging
        [JsonIgnore]
        public string RawResponse { get; set; }
    }
}