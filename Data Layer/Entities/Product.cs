using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer.Entities;

[Index("CategoryId", Name = "IX_Products_CategoryId")]
[Index("UnitId", Name = "IX_Products_UnitId")]
public partial class Product
{
    [Key]
    public int Id { get; set; }

    [StringLength(150)]
    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public int UnitId { get; set; }

    [StringLength(50)]
    public string? InternalCode { get; set; }

    public bool IsActive { get; set; }

    public bool IsSellable { get; set; }

    public bool IsPurchasable { get; set; }

    public bool DoesExpire { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("UnitId")]
    [InverseProperty("Products")]
    public virtual Unit Unit { get; set; } = null!;
}
