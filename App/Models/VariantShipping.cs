using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Models
{
    public class VariantShipping
    {
        [Range(0, 10000)]
        public double PerBox { get; set; }

        [Range(0, 10000)]
        public double PerRow { get; set; }

        [Range(0, 10000)]
        public double PerPallet { get; set; }
    }
}
