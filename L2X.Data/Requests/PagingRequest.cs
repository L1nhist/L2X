namespace L2X.Data.Requests;

public class PagingRequest : FilterRequest
{
    public int? PageIndex { get; set; } = 0;

    public int? PageSize { get; set; } = 50;
}