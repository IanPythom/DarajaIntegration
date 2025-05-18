namespace DarajaAPI.Models.Dto.Config
{
    public class DarajaConfig
    {
        public string Environment { get; set; }
        public DarajaUrlsConfig BaseUrls { get; set; }
        public DarajaEndpointsConfig Endpoints { get; set; }
        public DarajaSecurityConfig Security { get; set; }
        public DarajaCredentialsConfig Credentials { get; set; }
        public DarajaUrlConfig Urls { get; set; }
        public CertificateConfig Certificates { get; set; }
    }
}
