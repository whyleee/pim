using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Models
{
    public class MarkAsNew
    {
        public bool New { get; set; }

        [Range(1, 100)]
        public int? WeeksCount { get; set; }

        public DateTime? StartingFrom { get; set; }
    }
}
