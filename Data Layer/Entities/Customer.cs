using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer.Entities;

public partial class Customer
{
    [Key]
    public int Id { get; set; }

    [StringLength(150)]
    public string Name { get; set; } = null!;

    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(250)]
    public string? Address { get; set; }

    public bool IsWalkIn { get; set; }

    public DateTime CreatedAt { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
