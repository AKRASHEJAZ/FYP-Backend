using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data_Layer.enums;

namespace Data_Layer.Entities;

public partial class InventoryAction
{
    [Key]
    public int Id { get; set; }

    public int InventoryBatchId { get; set; }

    public InventoryActionType ActionType { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Quantity { get; set; }

    public int? ReferenceId { get; set; }

    public InventoryReferenceType ReferenceType { get; set; }

    [StringLength(250)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("InventoryActions")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("InventoryBatchId")]
    [InverseProperty("InventoryActions")]
    public virtual InventoryBatch InventoryBatch { get; set; } = null!;
}
