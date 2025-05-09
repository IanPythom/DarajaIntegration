using System.ComponentModel.DataAnnotations;

namespace DarajaAPI.Models.Domain
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public int Price { get; set; }
        public string RoomStatus { get; set; }
    }
    public enum RoomStatus
    {
        Available,
        Booked,
        Reserved
    }
}
