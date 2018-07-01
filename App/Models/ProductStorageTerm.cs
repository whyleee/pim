using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Models
{
    public class ProductStorageTerm
    {
        [Range(0, 10000)]
        public int? UnopenedOnSaleDays { get; set; }

        [Range(0, 10000)]
        public int? UnopenedTotalDays { get; set; }

        [Range(0, 10000)]
        public int? OpenedDays { get; set; }
    }
}
