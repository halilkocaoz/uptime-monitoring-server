using System;
using System.Text.Json.Serialization;
using UpMo.Common.Monitor;

namespace UpMo.Common.DTO.Request.Monitor
{
    public class MonitorUpdateRequest
    {
        [JsonIgnore]
        public Guid ID { get; set; }
        [JsonIgnore]
        public int AuthenticatedUserID { get; set; }

        public string Name { get; set; }
        public string Host { get; set; }

        public MonitorMethodType Method { get; set; }

        public MonitorCheckIntervalMs IntervalMs { get; set; }
    }
}