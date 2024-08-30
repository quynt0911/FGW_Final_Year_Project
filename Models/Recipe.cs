using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Blank.Models
{
    public class Recipe
    {
        [Key]
        public int RecipeId { get; set; }
        public string RecipeName { get; set; }
        public string ReDescription { get; set; }
    }
}