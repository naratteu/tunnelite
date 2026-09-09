#nullable disable
using MessagePack;
using System.Net.WebSockets;

namespace Tunnelite.Sdk;

/// <summary>
/// A chunk of WebSocket traffic on its way through the tunnel.
/// </summary>
/// <remarks>
/// The server binds this as a <c>(ReadOnlyMemory&lt;byte&gt;, WebSocketMessageType)</c> tuple, which
/// MessagePack writes as a two element array - hence the explicit keys rather than
/// <c>keyAsPropertyName</c>. It is a class rather than a tuple because SignalR resolves the type of a
/// stream item at runtime, and that resolution only works for reference types once the app is compiled
/// with NativeAOT.
/// </remarks>
[MessagePackObject]
public class WsChunk
{
    public WsChunk()
    {
    }

    public WsChunk(byte[] data, WebSocketMessageType type)
    {
        Data = data;
        Type = type;
    }

    [Key(0)]
    public byte[] Data { get; set; }

    [Key(1)]
    public WebSocketMessageType Type { get; set; }
}
