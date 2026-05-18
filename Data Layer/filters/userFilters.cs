namespace Data_Layer.filters;

public class UserFilters : Pagination   
{
    public List<int>? ID { get; set; }
    public string? Name { get; set; }
    public List<string>? Roles { get; set; }
    public bool? isActive { get; set; }
}

