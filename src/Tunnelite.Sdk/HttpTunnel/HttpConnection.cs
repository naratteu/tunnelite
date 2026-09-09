#nullable disable
using MessagePack;

namespace Tunnelite.Sdk;

[MessagePackObject(keyAsPropertyName: true)]
public class HttpConnection
{
    public Guid RequestId { get; set; }
    public string Method { get; set; }
    public string ContentType { get; set; }
    public string Path { get; set; }
}

[MessagePackObject(keyAsPropertyName: true)]
public class WsConnection
{
    public Guid RequestId { get; set; }
    public string Path { get; set; }
}

[MessagePackObject(keyAsPropertyName: true)]
public class SseConnection : HttpConnection
{
    public string Content { get; set; }
}
