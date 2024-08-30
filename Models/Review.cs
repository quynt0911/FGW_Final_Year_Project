using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Blank.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        public int RestaurantId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}