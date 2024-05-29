using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.API.Models
{
    public class Cart
    {
        public string CartId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public List<CartItem> CartItems { get; set; }
        public DateTime ExpirationTimeStamp { get; set; }
        public decimal TotalPrice
        {
            get
            {
                if (CartItems == null)
                {
                    return 0;
                }
                return CartItems.Sum(ci => ci.Price);
            }
        }
    }
}
