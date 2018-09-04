using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Models
{
    public class Dimensions
    {
        [Range(0, 100000)]
        public int Width { get; set; }

        [Range(0, 100000)]
        public int Height { get; set; }

        [Range(0, 100000)]
        public int Length { get; set; }
    }
}
