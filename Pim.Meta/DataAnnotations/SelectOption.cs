using System;
using System.Collections.Generic;
using System.Linq;

namespace Pim.Meta.DataAnnotations
{
    public class SelectOption
    {
        public SelectOption(string text, string value)
        {
            Text = text;
            Value = value;
        }

        public string Text { get; }
        public string Value { get; }
    }
}
