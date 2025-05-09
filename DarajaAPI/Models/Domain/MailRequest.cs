using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DarajaAPI.Models.Domain
{
    public class MailRequest
    {
        [Key]
        public Guid id { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        [NotMapped] // Since we don't need to store the Attachments in the database and they are only used for uploading files 
        public List<IFormFile> Attachments { get; set; }
    }
}