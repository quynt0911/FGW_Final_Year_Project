using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Blank.Models
{
    public class Restaurant
    {
        public int RestaurantId { get; set; }
        public string ResName { get; set; }
        public string Location { get; set; }
        public string Photo { get; set; }
    }
}