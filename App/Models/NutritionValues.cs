using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Models
{
    public class NutritionValues
    {
        [Range(0, 10000)]
        public double EnergyKcal { get; set; }

        [Range(0, 10000)]
        public double EnergyKj { get; set; }

        [Range(0, 100)]
        public double Fat { get; set; }

        [Range(0, 100)]
        public double SaturatedFat { get; set; }

        [Range(0, 100)]
        public double Carbohydrate { get; set; }

        [Range(0, 100)]
        public double Sugar { get; set; }

        [Range(0, 100)]
        public double Protein { get; set; }

        [Range(0, 100)]
        public double Salt { get; set; }

        [Range(0, 100000)]
        public double CalciumMg { get; set; }

        [Range(0, 100)]
        public double Lactose { get; set; }
    }
}
