using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Blank.Models
{
    public class Dish
    {
        [Key]
        public int DishId { get; set; }
        public int RecipeId { get; set; }
        public string DishName { get; set; }
        public string DDescription { get; set; }

    }
}