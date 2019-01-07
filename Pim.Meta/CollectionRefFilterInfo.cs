using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Pim.Meta
{
    [DataContract]
    public class CollectionRefFilterInfo : CollectionFilterInfo
    {
        [DataMember(EmitDefaultValue = false)]
        public string RefBackendKey { get; set; }

        [DataMember]
        public string RefCollectionKey { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Multiple { get; set; }

        [DataMember]
        public IDictionary<string, object> Filters { get; set; }
    }
}
