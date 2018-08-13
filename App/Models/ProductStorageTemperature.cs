using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Models
{
    public class ProductStorageTemperature
    {
        [Range(-50, 50)]
        public int? Min { get; set; }

        [Range(-50, 50)]
        public int? Max { get; set; }

        [Range(-50, 50)]
        public int? MaxOpened { get; set; }

        public bool? Freezing { get; set; }

        [Range(-50, 50)]
        public int? Recommended { get; set; }
    }
}
