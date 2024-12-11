namespace L2X.Data.Requests;

public class PagingRequest : FilterRequest
{
    public int? PageIndex { get; set; }

    public int? PageSize { get; set; }
}