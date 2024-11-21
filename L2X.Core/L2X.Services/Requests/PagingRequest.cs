namespace L2X.Services.Requests;

public class PagingRequest : FilterRequest
{
    public int? PageIndex { get; set; }

    public int? PageSize { get; set; }
}