using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Blank.Models
{
    public class Ingredient
    {
        [Key]
        public int IngId { get; set; }
        public string IngName { get; set; }
        public string IngDescription { get; set; }
    }
}