using System.ComponentModel.DataAnnotations;

namespace DarajaAPI.Models.Dto
{
    public class LipaNaSimulationRequest
    {
        [Required]
        [StringLength(20, MinimumLength = 20,
            ErrorMessage = "Account number must be 20 characters")]
        public string AccountNumber { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        [RegularExpression(@"^2547\d{8}$",
            ErrorMessage = "Phone number must be in format 2547XXXXXXXX")]
        public string PhoneNumber { get; set; }

        [StringLength(20, ErrorMessage = "Reference too long (max 20 chars)")]
        public string TransactionReference { get; set; }
    }
}