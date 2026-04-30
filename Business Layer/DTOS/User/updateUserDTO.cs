using System.ComponentModel.DataAnnotations;

namespace Business_Layer.DTOS;

public class UpdateUserDTO
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public bool? IsActive { get; set; }
    public int? RoleId { get; set; }
}

public class ChangePasswordDTO
{
    [Required]
    public string CurrentPassword { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string NewPassword { get; set; } = null!;
}