using System.ComponentModel.DataAnnotations;

namespace Aljmal.Models
{
    public class Register
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string PublicNaame { get; set; }
        public string Role { get; set; }
        public IFormFile image { get; set; }
        
        
    } 
}


