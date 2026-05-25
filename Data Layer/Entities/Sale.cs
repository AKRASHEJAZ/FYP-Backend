using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer.Entities;

public partial class Sale
{
    [Key]
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; }

    public DateTime SaleDate { get; set; }

    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("Sales")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("CustomerId")]
    [InverseProperty("Sales")]
    public virtual Customer? Customer { get; set; }
}
