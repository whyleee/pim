using System;
using System.Collections.Generic;

namespace Pim.Meta.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GetModelAttribute : Attribute
    {
        public GetModelAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
    }
}
