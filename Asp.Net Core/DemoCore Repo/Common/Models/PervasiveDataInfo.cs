using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class PervasiveDataInfo
    {
        [JsonProperty("PervasiveSQL")]
        public string PervasiveSQL { get; set; }
        [JsonProperty("DynamicParameters")]
        public Dictionary<string, object> DynamicParameters { get; set; }
        [JsonProperty("UseTransaction")]
        public bool UseTransaction { get; set; }
        [JsonProperty("MultipleQueriesAndParameters")]
        public List<PsqlQueryAndParameters> MultipleQueriesAndParameters { get; set; }
    }

    public class PsqlQueryAndParameters
    {
        [JsonProperty("PsqlCommandText")]
        public string PsqlCommandText { get; set; }
        [JsonProperty("DynamicParameters")]
        public Dictionary<string, object> DynamicParameters { get; set; }
    }
}
