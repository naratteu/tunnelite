#nullable disable
using System.Net.WebSockets;

namespace Tunnelite.Sdk;

/// <summary>
/// A chunk of WebSocket traffic on its way through the tunnel.
/// </summary>
/// <remarks>
/// The server binds this as a <c>(ReadOnlyMemory&lt;byte&gt;, WebSocketMessageType)</c> tuple. It is a
/// class here rather than that tuple because SignalR resolves the type of a stream item at runtime, and
/// under NativeAOT that resolution only works for reference types. The rest of the tunnel gets away
/// with plain <c>byte[]</c> for the same reason.
/// </remarks>
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

    public byte[] Data { get; set; }

    public WebSocketMessageType Type { get; set; }
}
