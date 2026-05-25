using Data_Layer.Entities;
using System.ComponentModel.DataAnnotations;

namespace Business_Layer.DTOS
{
    public class UserDto
    {
        public UserDto() { }

        public UserDto(User u)
        {
            this.Id = u.Id;
            this.Name = u.Name;
            this.Email = u.Email;
            this.CreatedAt = u.CreatedAt;
            this.IsActive = u.IsActive;
            this.RoleId = u.RoleId;
            if(u.Role != null)
            {
                this.Role = u.Role.Name;
            }
        }

        public int? Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public bool IsActive { get; set; }
    
        public string? Role { get; set; }

        public int? RoleId { get; set; }
    }
}
