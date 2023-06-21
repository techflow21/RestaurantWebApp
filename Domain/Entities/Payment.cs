
namespace Domain.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string CardNo { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int CvvNo { get; set; }
        public string Address { get; set; }
        public string PaymentMode { get; set; }
    }
}
