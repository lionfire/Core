using System.Runtime.Serialization;

namespace LionFire.Services
{
    [DataContract]
    public enum ServiceState
    {
        [EnumMember]
        Unspecified,
        [EnumMember]
        Started,
        [EnumMember]
        Starting,
        [EnumMember]
        Stopped,
        [EnumMember]
        Stopping,
        [EnumMember]
        Paused,
        [EnumMember]
        Pausing,
    }

}
