using System;
using System.Runtime.Serialization;

namespace NewBISReports.Models
{
    [DataContract]
    public class DeviceSearch
    {

        [DataMember(Name = "ClientId")]
        public string ClientId { get; set; }
        [DataMember(Name = "DeviceSearch")]
        public string DeviceToSearch { get; set; }
    }
}