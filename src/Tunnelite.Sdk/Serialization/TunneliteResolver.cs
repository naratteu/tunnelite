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
/// which emits IL at runtime and therefore throws under NativeAOT. This puts the formatters generated
/// from the <c>[MessagePackObject]</c> attributes in front of it, so no IL has to be emitted for any
/// type that actually travels over the tunnel. The generated formatters produce the same bytes the
/// contractless resolver did, so the wire format - and compatibility with an unmodified server - is
/// unchanged.
/// </remarks>
public static class TunneliteResolver
{
    public static readonly IFormatterResolver Instance = CompositeResolver.Create(
        [
            // SignalR puts DynamicEnumAsStringResolver in front of its chain, so enums go over the
            // wire as strings. That resolver emits IL as well; this is the static equivalent.
            new EnumAsStringFormatter<WebSocketMessageType>(),
        ],
        [
            GeneratedResolver.Instance,
            // Everything else (string, byte[], Guid, ...) is served by the built-in formatters.
            StandardResolver.Instance,
        ]);
}
