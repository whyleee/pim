using System;
using System.Collections.Generic;

namespace Pim.Meta.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CollectionRefFilterValueAttribute : Attribute
    {
        public CollectionRefFilterValueAttribute(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public object Value { get; }
    }
}
