using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Models
{
    public class Backend
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Variant> Variants { get; set; }
    }
}
