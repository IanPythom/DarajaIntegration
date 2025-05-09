using System.ComponentModel.DataAnnotations;

namespace DarajaAPI.Models.Dto
{
    public class PaymentRequestDto
    {
        [Required]
        public string? AdmissionNumber { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public string? MethodId { get; set; }
        [Required]
        public string? ChannelId { get; set; }
        [Required]
        public DateTimeOffset PaymentDate { get; set; }
    }
}