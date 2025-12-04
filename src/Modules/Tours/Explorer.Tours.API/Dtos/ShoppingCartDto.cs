using System.Collections.Generic;

namespace Explorer.Tours.API.Dtos
{
    public class ShoppingCartDto
    {
        public long Id { get; set; }
        public long TouristId { get; set; }
        public List<OrderItemDto> Items { get; set; }
        public double TotalPrice { get; set; }
    }
}
