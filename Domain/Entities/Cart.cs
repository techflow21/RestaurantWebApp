namespace Domain.Entities
{
    public class Cart
    {
        public int CartId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int ProductId { get; set; }
        public Menu Product { get; set; }
        public int Quantity { get; set; }
    }
}
