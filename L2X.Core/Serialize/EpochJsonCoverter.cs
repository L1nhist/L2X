namespace L2X.Core.Serialize;

public class EpochJsonConverter : JsonConverter<Epoch>
{
    public override Epoch Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        => long.TryParse(reader.GetString(), out long ticks) ? new(ticks) : Epoch.Zero;

    public override void Write(Utf8JsonWriter writer, Epoch epoch, JsonSerializerOptions options)
        => writer.WriteNumberValue(epoch.Timestamp);
}