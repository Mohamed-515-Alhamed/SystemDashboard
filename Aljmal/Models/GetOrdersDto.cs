namespace Aljmal.Models
{
    public class GetOrdersDto
    {

        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderName { get; set; }
        public string? CustomerName { get; set; }
        public string? SellerName { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
