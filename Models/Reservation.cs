using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Blank.Models
{
    public class Reservation
    {
        [Key]
        public int ReserId { get; set; }
        
        public int TableId { get; set; }
        public int RestaurantId { get; set; }
        public DateTime DateTime { get; set; }
        public string RStatus { get; set; }

    }
}