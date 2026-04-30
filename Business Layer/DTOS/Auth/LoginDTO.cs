
using System.ComponentModel.DataAnnotations;


namespace Business_Layer.DTOS
{
    public class LoginDto
    {
        
        [EmailAddress]
        [StringLength(150)]
        public required string Email { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }
    }
}
