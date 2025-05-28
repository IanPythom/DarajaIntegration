using DarajaAPI.Enums;

namespace DarajaAPI.Models.Dto
{
    public class TransactionStatusCallbackDto
    {
        public TransactionResult Result { get; set; }

        public class TransactionResult
        {
            public int ResultType { get; set; }
            public int ResultCode { get; set; }
            public string ResultDesc { get; set; }
            public string OriginatorConversationID { get; set; }
            public string ConversationID { get; set; }
            public string TransactionID { get; set; }
            public ResultParameters ResultParameters { get; set; }
        }

        public class ResultParameters
        {
            public List<ResultParameter> ResultParameter { get; set; }
        }

        public class ResultParameter
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}