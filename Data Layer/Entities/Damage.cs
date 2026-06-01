using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer.Entities;

public partial class Damage
{
    [Key]
    public int Id { get; set; }

    [Column("Damage_Date")]
    public DateTime DamageDate { get; set; }

    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("Damages")]
    public virtual User CreatedByNavigation { get; set; } = null!;
}
