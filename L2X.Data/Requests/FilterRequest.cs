namespace L2X.Data.Requests;

public class FilterRequest : IRequest
{
    public string? FilterBy { get; set; }

    public IEnumerable<string>? OrderBy { get; set; }
}