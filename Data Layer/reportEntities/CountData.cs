namespace Business_Layer.DTOS;

public class CountData
{
    public int TotalProducts { get; set; }
    public int ActiveProduct { get; set; }
    public UserCount UserData { get; set; } = new UserCount();
    public decimal TotalSale { get; set; }
    public decimal TotalDamages { get; set; }
    public decimal TotalReturns { get; set; }
}

public class UserCount
{
    public int TotalUsers { get; set; }
    public int ActiveUsers {  get; set; }
}