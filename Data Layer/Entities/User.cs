using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer.Entities;

[Index("Email", Name = "UQ__Users__A9D105343BDE9ABC", IsUnique = true)]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(150)]
    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Damage> Damages { get; set; } = new List<Damage>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<InventoryAction> InventoryActions { get; set; } = new List<InventoryAction>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    [InverseProperty("PerformedByNavigation")]
    public virtual ICollection<UserAuditLog> UserAuditLogPerformedByNavigations { get; set; } = new List<UserAuditLog>();

    [InverseProperty("User")]
    public virtual ICollection<UserAuditLog> UserAuditLogUsers { get; set; } = new List<UserAuditLog>();
}
