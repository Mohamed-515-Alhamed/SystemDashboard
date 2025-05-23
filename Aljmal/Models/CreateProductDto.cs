using System.ComponentModel.DataAnnotations;

namespace Aljmal.Models
{
    public class CreateProductDto
    {
       // public string subId { get; set; }
        [Required]
        public string? ProductName { get; set; }
        [Required]
        public decimal? Price { get; set; }
        public string? EnglishName { get; set; }
        public string? Detials { get; set; }
        [Required]
        public IFormFile image { get; set; }
    }
    public class UpdateProductDto
    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public decimal? Price { get; set; }
        public string? EnglishName { get; set; }
        public string? Detials { get; set; }

    }
}
