namespace LionFire.Stride_.Runtime;

[Flags]
public enum ClientOrServer : byte
{
    Unspecified = 0,
    Client = 1 << 1,
    Server = 1 << 2,
}

