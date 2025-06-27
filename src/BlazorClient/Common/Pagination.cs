namespace BlazorClient.Common;
public class Pagination
{
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public int PageSize { get; set; }
    internal bool HasPreviousPage => PageNumber > 1;
    
}
