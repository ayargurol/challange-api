using System.Runtime.Serialization;

namespace Challange.Core.Common
{
    [Serializable]
    [DataContract]
    public enum ServiceResponseStatuses
    {
        [EnumMember]
        Error,

        [EnumMember]
        Success,

        [EnumMember]
        Warning
    }
}
