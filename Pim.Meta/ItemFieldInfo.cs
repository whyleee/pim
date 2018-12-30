using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Pim.Meta
{
    [DataContract]
    public class ItemFieldInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Kind { get; set; }

        [DataMember]
        public IDictionary<string, object> Attributes { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ItemTypeInfo ComplexType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public CollectionRefInfo Ref { get; set; }
    }
}
