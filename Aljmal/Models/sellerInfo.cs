namespace Aljmal.Models
{
    public class sellerInfo
    {
        public string sellerId { get; set; }
        public string name { get; set; }
        public string logo { get; set; }
        public int? productCount { get; set; }
        public int? orderCount { get; set; }
        public IEnumerable<GetOrdersDto>? getOrdersDtos { get; set; }
    }
}
