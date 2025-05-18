using DarajaAPI.Models.Domain;
using DarajaAPI.Models.Dto;
using DarajaAPI.Models.Dto.Config;
using DarajaAPI.Records;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DarajaAPI.Services.Daraja
{
    public class DarajaPaymentService : IDarajaPaymentService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly DarajaSetting _settings;
        private readonly DarajaConfig _config;
        private readonly IDarajaAuthService _authService;
        private readonly ILogger<DarajaPaymentService> _logger;

        public DarajaPaymentService(IHttpClientFactory clientFactory, IOptions<DarajaSetting> settings, IOptions<DarajaConfig> config, IDarajaAuthService authService, ILogger<DarajaPaymentService> logger)
        {
            _clientFactory = clientFactory;
            _settings = settings.Value;
            _config = config.Value;
            _authService = authService;
            _logger = logger;
        }

        public async Task<object> ProcessPaymentAsync(string phoneNumber, decimal amount, string referenceNumber)
        {
            var token = await _authService.GetAccessTokenAsync();
            var client = _clientFactory.CreateClient("mpesa");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Use test credentials from Daraja
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var password = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_config.Credentials.ShortCode}{_config.Credentials.PassKey}{timestamp}"));

            var requestBody = new
            {
                BusinessShortCode = "600998", // From your test credentials
                Password = password,
                Timestamp = timestamp,
                TransactionType = "CustomerPayBillOnline", // Recommended for C2B
                Amount = amount,
                PartyA = phoneNumber, // Use 254708374149 for sandbox testing
                PartyB = "600998", // Should match BusinessShortCode
                PhoneNumber = phoneNumber, // Use 254708374149
                AccountReference = referenceNumber,
                TransactionDesc = "Payment to IAN MWENDA GATUMU",
                CallBackURL = _config.Urls.Confirmation // Must be HTTPS
            };

            try
            {
                var response = await client.PostAsJsonAsync(
                    "https://sandbox.safaricom.co.ke/mpesa/stkpush/v1/processrequest",
                    requestBody
                );

                _logger.LogInformation("Payment processed: {Phone}, {Amount}", phoneNumber, amount);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment processing failed for {Phone}", phoneNumber);
                throw;
            }
        }

        public async Task<string> SimulateC2BPaymentAsync(C2BRequestDto request)
        {
            var token = await _authService.GetAccessTokenAsync();
            var client = _clientFactory.CreateClient("mpesa");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var requestBody = new
            {
                ShortCode = _settings.ShortCode,
                CommandID = "CustomerPayBillOnline",
                Amount = request.Amount,
                Msisdn = request.PhoneNumber,
                BillRefNumber = request.AccountNumber
            };

            var response = await client.PostAsJsonAsync(_settings.C2BSimulateUrl, requestBody);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<TransactionStatusResult> VerifyTransactionAsync(string transactionId)
        {
            var settings = _settings;
            var token = await _authService.GetAccessTokenAsync();
            var client = _clientFactory.CreateClient("mpesa");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var password = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{settings.ShortCode}{settings.PassKey}{timestamp}"));

            var baseUrl = GetBaseUrl();

            var request = new
            {
                Initiator = settings.InitiatorName,
                SecurityCredential = GenerateSecurityCredential(settings.InitiatorPassword),
                CommandID = "TransactionStatusQuery",
                TransactionID = transactionId,
                PartyA = settings.ShortCode,
                IdentifierType = "4",
                ResultURL = $"{baseUrl}/api/v1/mpesa/transaction-status",
                QueueTimeOutURL = $"{settings.BaseUrl}/api/v1/mpesa/timeout",
                Remarks = "Transaction Verification",
                Occasion = "VerifyTransaction"
            };

            var response = await client.PostAsJsonAsync(settings.TransactionStatusEndpoint, request);
            var content = await response.Content.ReadAsStringAsync();

            return ParseTransactionStatus(content);
        }

        private string GenerateSecurityCredential(string initiatorPassword)
        {
            var cert = new X509Certificate2(Path.Combine("certs", "prod_cert.cer"));
            using var rsa = cert.GetRSAPublicKey();
            return Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(initiatorPassword), RSAEncryptionPadding.Pkcs1));
        }

        private TransactionStatusResult ParseTransactionStatus(string responseContent)
        {
            var json = JObject.Parse(responseContent);
            return new TransactionStatusResult(
                json["ResultCode"]?.Value<string>() == "0",
                json["ResultCode"]?.Value<string>(),
                json["ResultDesc"]?.Value<string>(),
                json["TransactionStatus"]?.Value<string>()
            );
        }

        private string GetBaseUrl()
        {
            return _config.Environment.Equals("Production", StringComparison.OrdinalIgnoreCase)
                ? _config.BaseUrls.Production
                : _config.BaseUrls.Sandbox;
        }
    }
}
