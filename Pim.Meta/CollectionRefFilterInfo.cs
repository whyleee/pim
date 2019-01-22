using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Pim.Meta
{
    [DataContract]
    public class CollectionRefFilterInfo : CollectionFilterInfo
    {
        [DataMember(EmitDefaultValue = false)]
        public bool Multiple { get; set; }

        [DataMember]
        public object Ref { get; set; }
    }
}
