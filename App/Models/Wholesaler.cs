using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Models
{
    public class Wholesaler
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
