using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using System.Net.WebSockets;

namespace Tunnelite.Sdk;

/// <summary>
/// The MessagePack resolver the tunnel hub connections use.
/// </summary>
/// <remarks>
/// SignalR's default chain resolves user types through <see cref="ContractlessStandardResolver"/>,
/// which emits IL at runtime and therefore throws under NativeAOT. These formatters produce the same
/// bytes without emitting anything, so the wire format - and compatibility with an unmodified server -
/// is unchanged. SignalR's chain stays on as a fallback: reaching it means a type went over the hub
/// without a formatter here, which works under the JIT and fails loudly under NativeAOT.
/// </remarks>
public static class TunneliteResolver
{
    public static readonly IFormatterResolver Instance = CompositeResolver.Create(
        [
            new HttpConnectionFormatter(),
            new SseConnectionFormatter(),
            new WsConnectionFormatter(),
            new WsChunkFormatter(),
            new TcpConnectionFormatter(),
            new TcpTunnelRequestFormatter(),
            new TcpTunnelResponseFormatter(),
            // SignalR puts DynamicEnumAsStringResolver in front of its chain, so enums travel as
            // strings. This is the static equivalent.
            new EnumAsStringFormatter<WebSocketMessageType>(),
        ],
        [
            DynamicEnumAsStringResolver.Instance,
            ContractlessStandardResolver.Instance,
        ]);
}

internal sealed class HttpConnectionFormatter : MapFormatter<HttpConnection>
{
    protected override int MemberCount => 4;

    protected override void WriteMembers(ref MessagePackWriter writer, HttpConnection value)
    {
        writer.Write(nameof(value.RequestId));
        Write(ref writer, value.RequestId);
        writer.Write(nameof(value.Method));
        writer.Write(value.Method);
        writer.Write(nameof(value.ContentType));
        writer.Write(value.ContentType);
        writer.Write(nameof(value.Path));
        writer.Write(value.Path);
    }

    protected override void ReadMember(ref MessagePackReader reader, HttpConnection value, string? key)
    {
        switch (key)
        {
            case nameof(value.RequestId): value.RequestId = ReadGuid(ref reader); break;
            case nameof(value.Method): value.Method = reader.ReadString(); break;
            case nameof(value.ContentType): value.ContentType = reader.ReadString(); break;
            case nameof(value.Path): value.Path = reader.ReadString(); break;
            default: reader.Skip(); break;
        }
    }
}

internal sealed class SseConnectionFormatter : MapFormatter<SseConnection>
{
    protected override int MemberCount => 5;

    protected override void WriteMembers(ref MessagePackWriter writer, SseConnection value)
    {
        // The contractless resolver emits the inherited members first, so this order is what the
        // server produces for the same object.
        writer.Write(nameof(value.RequestId));
        Write(ref writer, value.RequestId);
        writer.Write(nameof(value.Method));
        writer.Write(value.Method);
        writer.Write(nameof(value.ContentType));
        writer.Write(value.ContentType);
        writer.Write(nameof(value.Path));
        writer.Write(value.Path);
        writer.Write(nameof(value.Content));
        writer.Write(value.Content);
    }

    protected override void ReadMember(ref MessagePackReader reader, SseConnection value, string? key)
    {
        switch (key)
        {
            case nameof(value.Content): value.Content = reader.ReadString(); break;
            case nameof(value.RequestId): value.RequestId = ReadGuid(ref reader); break;
            case nameof(value.Method): value.Method = reader.ReadString(); break;
            case nameof(value.ContentType): value.ContentType = reader.ReadString(); break;
            case nameof(value.Path): value.Path = reader.ReadString(); break;
            default: reader.Skip(); break;
        }
    }
}

internal sealed class WsConnectionFormatter : MapFormatter<WsConnection>
{
    protected override int MemberCount => 2;

    protected override void WriteMembers(ref MessagePackWriter writer, WsConnection value)
    {
        writer.Write(nameof(value.RequestId));
        Write(ref writer, value.RequestId);
        writer.Write(nameof(value.Path));
        writer.Write(value.Path);
    }

    protected override void ReadMember(ref MessagePackReader reader, WsConnection value, string? key)
    {
        switch (key)
        {
            case nameof(value.RequestId): value.RequestId = ReadGuid(ref reader); break;
            case nameof(value.Path): value.Path = reader.ReadString(); break;
            default: reader.Skip(); break;
        }
    }
}

internal sealed class TcpConnectionFormatter : MapFormatter<TcpConnection>
{
    protected override int MemberCount => 1;

    protected override void WriteMembers(ref MessagePackWriter writer, TcpConnection value)
    {
        writer.Write(nameof(value.RequestId));
        Write(ref writer, value.RequestId);
    }

    protected override void ReadMember(ref MessagePackReader reader, TcpConnection value, string? key)
    {
        switch (key)
        {
            case nameof(value.RequestId): value.RequestId = ReadGuid(ref reader); break;
            default: reader.Skip(); break;
        }
    }
}

internal sealed class TcpTunnelRequestFormatter : MapFormatter<TcpTunnelRequest>
{
    protected override int MemberCount => 6;

    protected override void WriteMembers(ref MessagePackWriter writer, TcpTunnelRequest value)
    {
        writer.Write(nameof(value.LocalPort));
        writer.Write(value.LocalPort);
        writer.Write(nameof(value.PublicPort));
        if (value.PublicPort.HasValue)
        {
            writer.Write(value.PublicPort.Value);
        }
        else
        {
            writer.WriteNil();
        }

        writer.Write(nameof(value.Host));
        writer.Write(value.Host);
        writer.Write(nameof(value.ClientId));
        Write(ref writer, value.ClientId);
        writer.Write(nameof(value.LocalUrl));
        writer.Write(value.LocalUrl);
        writer.Write(nameof(value.PublicUrl));
        writer.Write(value.PublicUrl);
    }

    protected override void ReadMember(ref MessagePackReader reader, TcpTunnelRequest value, string? key)
    {
        switch (key)
        {
            case nameof(value.LocalPort): value.LocalPort = reader.ReadInt32(); break;
            case nameof(value.PublicPort): value.PublicPort = reader.TryReadNil() ? null : reader.ReadInt32(); break;
            case nameof(value.Host): value.Host = reader.ReadString(); break;
            case nameof(value.ClientId): value.ClientId = ReadGuid(ref reader); break;
            case nameof(value.LocalUrl): value.LocalUrl = reader.ReadString(); break;
            case nameof(value.PublicUrl): value.PublicUrl = reader.ReadString(); break;
            default: reader.Skip(); break;
        }
    }
}

internal sealed class TcpTunnelResponseFormatter : MapFormatter<TcpTunnelResponse>
{
    protected override int MemberCount => 4;

    protected override void WriteMembers(ref MessagePackWriter writer, TcpTunnelResponse value)
    {
        writer.Write(nameof(value.TunnelUrl));
        writer.Write(value.TunnelUrl);
        writer.Write(nameof(value.Port));
        writer.Write(value.Port);
        writer.Write(nameof(value.Error));
        writer.Write(value.Error);
        writer.Write(nameof(value.Message));
        writer.Write(value.Message);
    }

    protected override void ReadMember(ref MessagePackReader reader, TcpTunnelResponse value, string? key)
    {
        switch (key)
        {
            case nameof(value.TunnelUrl): value.TunnelUrl = reader.ReadString(); break;
            case nameof(value.Port): value.Port = reader.ReadInt32(); break;
            case nameof(value.Error): value.Error = reader.ReadString(); break;
            case nameof(value.Message): value.Message = reader.ReadString(); break;
            default: reader.Skip(); break;
        }
    }
}

/// <summary>
/// <see cref="WsChunk"/> stands in for the <c>(ReadOnlyMemory&lt;byte&gt;, WebSocketMessageType)</c>
/// tuple the server binds, which MessagePack writes as a two element array.
/// </summary>
internal sealed class WsChunkFormatter : IMessagePackFormatter<WsChunk?>
{
    public void Serialize(ref MessagePackWriter writer, WsChunk? value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        writer.WriteArrayHeader(2);
        writer.Write(value.Data);
        writer.Write(value.Type.ToString());
    }

    public WsChunk? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }

        options.Security.DepthStep(ref reader);

        try
        {
            var count = reader.ReadArrayHeader();
            var value = new WsChunk();

            for (var i = 0; i < count; i++)
            {
                switch (i)
                {
                    case 0: value.Data = reader.ReadBytes() is { } bytes ? System.Buffers.BuffersExtensions.ToArray(bytes) : []; break;
                    case 1: value.Type = Enum.Parse<WebSocketMessageType>(reader.ReadString()!); break;
                    default: reader.Skip(); break;
                }
            }

            return value;
        }
        finally
        {
            reader.Depth--;
        }
    }
}
