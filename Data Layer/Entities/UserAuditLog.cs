using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer.Entities;

public partial class UserAuditLog
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    [StringLength(50)]
    public string Action { get; set; } = null!;

    public int? PerformedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime PerformedAt { get; set; }

    public string? Details { get; set; }

    [ForeignKey("PerformedBy")]
    [InverseProperty("UserAuditLogPerformedByNavigations")]
    public virtual User? PerformedByNavigation { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserAuditLogUsers")]
    public virtual User User { get; set; } = null!;
}
