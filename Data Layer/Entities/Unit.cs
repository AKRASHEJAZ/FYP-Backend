using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer.Entities;

public partial class Unit
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(20)]
    public string Symbol { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [InverseProperty("Unit")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
