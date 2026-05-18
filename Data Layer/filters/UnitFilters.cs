namespace Data_Layer.filters;

public class UnitFilters : Pagination
{
    public int Id { get; set; }

    public IList<string>? Name { get; set; }

    public IList<string>? Symbol { get; set; }
}
