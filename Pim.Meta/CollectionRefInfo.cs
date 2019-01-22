using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Pim.Meta
{
    [DataContract]
    public class CollectionRefInfo
    {
        [DataMember(EmitDefaultValue = false)]
        public string BackendKey { get; set; }

        [DataMember]
        public string CollectionKey { get; set; }

        [DataMember]
        public IDictionary<string, object> Filters { get; set; }
    }
}
