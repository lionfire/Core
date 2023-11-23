namespace LionFire.LionRing
{
    public enum ConnectionState
    {
        Unspecified,
        Connected, // Heartbeats
        Connecting,

        //SemiConnected, // Occasionally connected

        Disconnecting,
        Disconnected,
        Failed, // User can initiate reconnect, but the other end might be gone!
    }
}
