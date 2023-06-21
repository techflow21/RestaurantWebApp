using Domain.Entities;

namespace Contracts.DTOs
{
    public class ReservationRequestDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime Date { get; set; }
        //public TimeSpan? Time { get; set; }
        public string Duration { get; set; }
        public int Guests { get; set; }
        public string? Menu { get; set; }
    }
}
