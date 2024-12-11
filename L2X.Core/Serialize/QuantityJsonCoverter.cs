namespace L2X.Core.Serialize;

public class QuantityJsonConverter : JsonConverter<Sequence>
{
    public override Sequence Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        => new(reader.GetString());

    public override void Write(Utf8JsonWriter writer, Sequence quantity, JsonSerializerOptions options)
        => writer.WriteRawValue(quantity.ToString());
}