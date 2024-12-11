namespace L2X.Core.Serialize;

public class UuidJsonConverter : JsonConverter<Uuid>
{
    public override Uuid Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        => new(reader.GetString());

    public override void Write(Utf8JsonWriter writer, Uuid guid, JsonSerializerOptions options)
        => writer.WriteStringValue(guid.ToString());
}