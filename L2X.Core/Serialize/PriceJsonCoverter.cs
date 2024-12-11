namespace L2X.Core.Serialize;

public class PriceJsonConverter : JsonConverter<Price>
{
    public override Price Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        => new(reader.GetString());

    public override void Write(Utf8JsonWriter writer, Price price, JsonSerializerOptions options)
        => writer.WriteStringValue(price.ToString());
}