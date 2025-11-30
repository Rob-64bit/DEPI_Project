using System.ComponentModel.DataAnnotations;

namespace Bookify.wep.Models.Bookings
{
    public class BookingCreateViewModel
    {
        public int RoomId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }

        [Range(1, 10)]
        public int Guests { get; set; } = 1;

        [Required] public string FullName { get; set; } = "";
        [Required][EmailAddress] public string Email { get; set; } = "";
        [Required] public string Phone { get; set; } = "";
        public string SpecialRequests { get; set; } = "";

        public decimal RoomPrice { get; set; }
        public decimal TotalPrice { get; set; }




    }
}
