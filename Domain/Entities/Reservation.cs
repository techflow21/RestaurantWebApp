
namespace Domain.Entities
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? Time { get; set; }
        public string Duration { get; set; }
        public int Guests { get; set; }
        public Menu? Menu { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Status { get; set; }
    }
}
