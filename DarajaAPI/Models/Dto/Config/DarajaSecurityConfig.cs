namespace DarajaAPI.Models.Dto.Config
{
    public class DarajaSecurityConfig
    {
        public string[] IPWhitelist { get; set; }
        public int TimeoutSeconds { get; set; }
        public InitiatorConfig Initiator { get; set; }
    }
}
