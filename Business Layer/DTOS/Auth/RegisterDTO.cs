
using System.ComponentModel.DataAnnotations;

namespace Business_Layer.DTOS.Auth
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public int RoleId { get; set; }
    }
}
