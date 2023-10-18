namespace LionFire.Net;

public static class PortConfiguration
{
    public static int? GetPortOrOffset(int? absolutePort, int? basePort, int? portOffset)
    {
        return absolutePort ?? (basePort.HasValue && portOffset.HasValue ? basePort.Value + portOffset.Value : null);
    }
}
