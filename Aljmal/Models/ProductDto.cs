namespace Aljmal.Models
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public decimal? Price { get; set; }
        public string? EnglishName { get; set; }
        public string? imagePath { get; set; }
        public string? Detials { get; set; }
    }
}
