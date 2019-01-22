using System;
using System.Collections.Generic;

namespace Pim.Meta.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CollectionRefFilterValueAttribute : Attribute
    {
        public CollectionRefFilterValueAttribute(string refKey, string key, object value)
        {
            RefKey = refKey;
            Key = key;
            Value = value;
        }

        public string RefKey { get; }
        public string Key { get; }
        public object Value { get; }
    }
}
