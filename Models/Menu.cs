using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Blank.Models
{
    public class Menu
    {
        [Key]
        public int MenuId { get; set; }
        public int DishId { get; set; }
        public int RestaurantId { get; set; }
    }
}