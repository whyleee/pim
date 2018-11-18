using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Pim.Meta
{
    [DataContract]
    public class CollectionInfo
    {
        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Readonly { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ItemsProperty { get; set; }

        [DataMember]
        public ItemTypeInfo ItemType { get; set; }

        [DataMember]
        public IEnumerable<CollectionFilterInfo> Filters { get; set; }
    }
}
