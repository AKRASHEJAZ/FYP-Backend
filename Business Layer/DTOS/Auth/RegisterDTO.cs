
using System.ComponentModel.DataAnnotations;

namespace Business_Layer.DTOS
{
    public class RegisterDto
    {
        [StringLength(100)]
        public required string Name { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public required string Email { get; set; }

        [MinLength(6)]
        public required string Password { get; set; }

        [Required]
        public int RoleId { get; set; }
    }
}
