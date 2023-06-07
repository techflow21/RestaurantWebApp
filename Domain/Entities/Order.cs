
namespace Domain.Entities
{
    public class Order
    {
        public int OrderId { get; set;}
        public string OrderNo { get; set;}
        public int ProductId { get; set;}
        public Product Product { get; set;}
        public int Quantity { get; set;}
        public string UserId { get; set;}
        public User User { get; set;}
        public string Status { get; set;}
        public int PaymentId { get; set;}
        public DateTime OrderDate { get; set;}
    }
}
