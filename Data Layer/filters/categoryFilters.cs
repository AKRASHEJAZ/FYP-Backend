namespace Data_Layer.filters;

public class CategoryFilters : Pagination
{
    public int Id { get; set; }
    public IList<string>? Name { get; set; }
}
