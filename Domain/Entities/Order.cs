
namespace Domain.Entities
{
    public class Order
    {
        public int OrderId { get; set;}
        public string OrderNo { get; set;}
        public int MenuId { get; set;}
        public Menu Menu { get; set;}
        public int Quantity { get; set;}
        public string UserId { get; set;}
        public User User { get; set;}
        public string Status { get; set;}
        public int? PaymentId { get; set;}
        public Payment? Payment { get; set;}
        public DateTime OrderDate { get; set;}
    }
}
