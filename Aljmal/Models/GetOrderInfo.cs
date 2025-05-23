namespace Aljmal.Models
{
    public class GetOrderInfo
    {
        public int OrderId { get; set; }
        public string? ProductName { get; set; }
        public int? Count { get; set; }
        public decimal UnitPrice { get; set; }
        //public decimal TotalPrice { get; set; }


    }
}
