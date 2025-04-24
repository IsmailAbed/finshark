namespace api.Helpers
{
    public class QueryObject
    {

        // for filtering
        public string? Symbol { get; set; } = null;
        // for filtering
        public string? CompanyName { get; set; } = null;


        public string? SortBy { get; set; } = null;

        public bool IsDecsending { get; set; } = false;

        
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
 
    }
}