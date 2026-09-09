#nullable disable
using MessagePack;

namespace Tunnelite.Sdk;

[MessagePackObject(keyAsPropertyName: true)]
public class TcpTunnelResponse
{
    public string TunnelUrl { get; set; }
    public int Port { get; set; }
    public string Error { get; set; }
    public string Message { get; set; }
}
