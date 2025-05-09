namespace DarajaAPI.Models.DTOs
{
    public class PaymentDto
    {
        public Guid PaymentID { get; set; }
        public double Amount { get; set; }
        //public string? PaymentMethod { get; set; }
        //public Method? PaymentMethod { get; set; }
        //public string? PaymentChannel { get; set; }
        //public Channel? PaymentChannel { get; set; }
        //public string? Student { get; set; }
        //public Student? Student { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTimeOffset PaymentDate { get; set; }
    }
}