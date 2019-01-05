using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Pim.Meta
{
    [DataContract]
    public abstract class CollectionFilterInfo
    {
        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Required { get; set; }
    }
}
