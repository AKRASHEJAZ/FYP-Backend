using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer.Entities;

public partial class InventoryBatch
{
    [Key]
    public int Id { get; set; }

    public int ProductId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal PurchasePrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SellingPrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Quantity { get; set; }

    [StringLength(100)]
    public string? BatchCode { get; set; }

    [Column("MFGDate")]
    public DateOnly? Mfgdate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("InventoryBatches")]
    public virtual Product Product { get; set; } = null!;
}
