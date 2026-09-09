#nullable disable
using MessagePack;

namespace Tunnelite.Sdk;

[MessagePackObject(keyAsPropertyName: true)]
public class HttpTunnelRequest
{
    public string Subdomain { get; set; }
    public Guid? ClientId { get; set; }
    public string LocalUrl { get; set; }
    public string PublicUrl { get; set; }
}
