#nullable disable
using MessagePack;

namespace Tunnelite.Sdk;

[MessagePackObject(keyAsPropertyName: true)]
public class TcpConnection
{
    public Guid RequestId { get; set; }
}
