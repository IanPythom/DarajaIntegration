using System.ComponentModel.DataAnnotations;

namespace DarajaAPI.Models.Domain
{
    public class WelcomeRequest
    {
        [Key]
        public Guid id { get; set;}
        public string ToEmail { get; set; }
        public string UserName { get; set; }
    }
}
