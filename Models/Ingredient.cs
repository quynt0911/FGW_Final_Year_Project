using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blank.Models
{
    public class Ingredient
    {
        [Key]
        public int IngId { get; set; }
        public string IngName { get; set; }
        public string IngDescription { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Number must be a positive value!")]
        public int Numer { get; set; }
        public string Photo { get; set; }
    }
}