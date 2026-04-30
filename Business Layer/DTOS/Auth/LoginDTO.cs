
using System.ComponentModel.DataAnnotations;


namespace Business_Layer.DTOS.Auth
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
