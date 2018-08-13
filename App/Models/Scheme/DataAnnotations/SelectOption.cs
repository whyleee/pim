using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Models.Scheme.DataAnnotations
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
