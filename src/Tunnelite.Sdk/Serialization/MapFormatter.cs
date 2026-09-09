using MessagePack;
using MessagePack.Formatters;

namespace Tunnelite.Sdk;

/// <summary>
/// Base class for the tunnel's MessagePack formatters.
/// </summary>
/// <remarks>
/// MessagePack's contractless resolver - the one SignalR uses by default - writes an object as a map
/// keyed by property name, and that is the shape the server reads. Only the members differ from one
/// type to the next, so the nil handling, the map header and the read loop live here and each formatter
/// is left with just its members.
/// </remarks>
internal abstract class MapFormatter<T> : IMessagePackFormatter<T?>
    where T : class, new()
{
    protected abstract int MemberCount { get; }

    protected abstract void WriteMembers(ref MessagePackWriter writer, T value);

    protected abstract void ReadMember(ref MessagePackReader reader, T value, string? key);

    public void Serialize(ref MessagePackWriter writer, T? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteMapHeader(MemberCount);

        WriteMembers(ref writer, value);
    }

    public T? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        options.Security.DepthStep(ref reader);

        try
        {
            var value = new T();
            var count = reader.ReadMapHeader();

            for (var i = 0; i < count; i++)
            {
                ReadMember(ref reader, value, reader.ReadString());
            }

            return value;
        }
        finally
        {
            reader.Depth--;
        }
    }

    /// <summary>Matches the built-in Guid formatter, which writes the 36 character form as a string.</summary>
    protected static void Write(ref MessagePackWriter writer, Guid value) => writer.Write(value.ToString());

    protected static Guid ReadGuid(ref MessagePackReader reader) =>
        reader.TryReadNil() ? Guid.Empty : Guid.Parse(reader.ReadString()!);
}
