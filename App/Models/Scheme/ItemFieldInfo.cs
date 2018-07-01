using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace App.Models.Scheme
{
    public class ItemFieldInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; }
        public IDictionary<string, object> Attributes { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ItemTypeInfo TypeMeta { get; set; }
    }
}
