using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer.Entities;

public partial class Return
{
    [Key]
    public int Id { get; set; }

    [Column("Sale_Id")]
    public int SaleId { get; set; }

    [Column("Customer_Id")]
    public int CustomerId { get; set; }

    [Column("Return_Date")]
    public DateTime ReturnDate { get; set; }

    public int CreatedBy { get; set; }

    [StringLength(250)]
    public string? Reason { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("Returns")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [ForeignKey("CustomerId")]
    [InverseProperty("Returns")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("SaleId")]
    [InverseProperty("Returns")]
    public virtual Sale Sale { get; set; } = null!;
}
