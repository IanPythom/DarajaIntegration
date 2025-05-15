namespace DarajaAPI.Models.Domain
{
    public class DarajaSecurityConfig
    {
        public string[] IPWhitelist { get; set; }
        public int TimeoutSeconds { get; set; }
        public InitiatorConfig Initiator { get; set; }
    }
}
