namespace L2X.Core.Serialize;

public class VolumeJsonConverter : JsonConverter<Volume>
{
    public override Volume Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        => new(reader.GetString());

    public override void Write(Utf8JsonWriter writer, Volume quantity, JsonSerializerOptions options)
        => writer.WriteRawValue(quantity.ToString());
}