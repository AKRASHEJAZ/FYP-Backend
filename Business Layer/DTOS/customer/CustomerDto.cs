using Data_Layer.Entities;

namespace Business_Layer.DTOS;

public class CustomerDto
{
    public CustomerDto() { }
    public CustomerDto(Customer c)
    {
        Id = c.Id;
        Name = c.Name;
        Phone = c.Phone;
        Email = c.Email;
        Address = c.Address;
        IsWalkIn = c.IsWalkIn;
        CreatedAt = c.CreatedAt;
    }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public bool IsWalkIn { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public bool IsWalkIn { get; set; }
}