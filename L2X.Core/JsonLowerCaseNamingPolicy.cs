namespace L2X.Core;

public class LowerCaseNamingPolicy : JsonNamingPolicy
{
	public override string ConvertName(string name) =>
		name.ToLower();
}