namespace L2X.Core.Serialize;

public class QuantityJsonConverter : JsonConverter<Quantity>
{
    public override Quantity Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        => new(reader.GetString());

    public override void Write(Utf8JsonWriter writer, Quantity quantity, JsonSerializerOptions options)
        => writer.WriteRawValue(quantity.ToString());
}