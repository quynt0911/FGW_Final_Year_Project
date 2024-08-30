using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blank.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int DishId { get; set; }
        public int TableId { get; set; }
        public string OStatus { get; set; }
    }
}