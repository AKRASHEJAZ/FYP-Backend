using Data_Layer.Entities;
using Data_Layer.enums;

namespace Business_Layer.DTOS;

public class InventoryActionDto
{
    public InventoryActionDto() { }
    public InventoryActionDto(InventoryAction a)
    {
        this.Id = a.Id;
        this.InventoryBatchId = a.InventoryBatchId;
        this.CreatedAt = a.CreatedAt;
        this.ReferenceId = a.ReferenceId;
        this.ReferenceType = a.ReferenceType;
        this.ActionType = a.ActionType;
        this.Quantity = a.Quantity;
        this.Notes = a.Notes;

        if(a.InventoryBatch != null)
        {
            this.InventoryBatch = new InventoryBatchDto(a.InventoryBatch);
        }
    }

    public int Id { get; set; }
    public int InventoryBatchId { get; set; }
    public InventoryActionType ActionType { get; set; }
    public decimal Quantity { get; set; }
    public int? ReferenceId { get; set; }
    public InventoryReferenceType ReferenceType { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    //Navigation Properties
    public InventoryBatchDto InventoryBatch { get; set; } = null!;
}

public class SaleDto
{
    public SaleDto() { }

    public SaleDto(Sale s, IList<InventoryAction> i) 
    {
        this.Id = s.Id;
        this.CustomerId = s.CustomerId;
        this.TotalAmount = s.TotalAmount;
        this.SaleDate = s.SaleDate;
        
        if(s.CreatedByNavigation != null)
        {
            this.CreatedBy = new UserDto(s.CreatedByNavigation);
        }

        if (s.Customer != null)
        {
            this.Customer = new CustomerDto(s.Customer);
        }
        if(i != null)
        {
            this.Actions = i.Select(a => new InventoryActionDto(a)).ToList();
        }
    }

    public int Id { get; set; }
    public int? CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime SaleDate { get; set; }

    //Navigation Properties
    public UserDto CreatedBy { get; set; } = null!;
    public virtual CustomerDto Customer { get; set; } = null!;
    public IList<InventoryActionDto> Actions { get; set; } = new List<InventoryActionDto>();
}

public class DamageDto
{
    public DamageDto() { }
    public DamageDto(Damage d, IList<InventoryAction> i)
    {
        this.Id = d.Id;
        this.DamageDate = d.DamageDate;
        if (d.CreatedByNavigation != null)
        {
            this.CreatedBy = new UserDto(d.CreatedByNavigation);
        }
        if (i != null)
        {
            this.Actions = i.Select(a => new InventoryActionDto(a)).ToList();
        }
    }
    public int Id { get; set; }
    public DateTime DamageDate { get; set; }
    //Navigation Properties
    public UserDto CreatedBy { get; set; } = null!;
    public IList<InventoryActionDto> Actions { get; set; } = new List<InventoryActionDto>();
}

public class ReturnDto
{
    public ReturnDto() { }

    public ReturnDto(Return r, IList<InventoryAction> i)
    {
        this.Id = r.Id;
        this.SaleId = r.SaleId;
        this.CustomerId = r.CustomerId;
        this.ReturnDate = r.ReturnDate;
        this.Reason = r.Reason;

        if(i != null)
        {
            this.Actions = i.Select(a => new InventoryActionDto(a)).ToList();
        }
    }

    public int Id { get; set; }
    public int SaleId { get; set; }
    public int CustomerId { get; set; }
    public DateTime ReturnDate { get; set; }
    public string? Reason { get; set; }
    public IList<InventoryActionDto> Actions { get; set; } = new List<InventoryActionDto>();

}