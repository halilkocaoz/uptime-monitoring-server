using System;
using System.Text.Json.Serialization;

namespace UpMo.Common.DTO.Request
{
    public class PostFormDataCreateRequest
    {
        [JsonIgnore]
        public Guid ID { get; private set; } = Guid.NewGuid();
        [JsonIgnore]
        public Guid MonitorID { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}