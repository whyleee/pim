using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pim.Meta.DataAnnotations;

namespace App.Models
{
    public class Backend
    {
        public IEnumerable<Product> Products { get; set; }

        [CollectionRefFilter("productId", nameof(Products))]
        public IEnumerable<Variant> Variants { get; set; }
    }
}
