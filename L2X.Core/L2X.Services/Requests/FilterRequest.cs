namespace L2X.Services.Requests;

public class FilterRequest : IRequest
{
    public string? FilterBy { get; set; }

    public IEnumerable<string>? OrderBy { get; set; }
}