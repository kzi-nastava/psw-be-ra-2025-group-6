namespace Explorer.Tours.API.Dtos
{
    public class OrderItemDto
    {
        public long TourId { get; set; }
        public string TourName { get; set; }
        public double Price { get; set; }
    }
}
