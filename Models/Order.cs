using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Blank.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int DishId { get; set; }
        public virtual Dish Dish { get; set; }

        [Required]
        public int TableId { get; set; }
        public virtual Table Table { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Total price must be non-negative.")]
        public decimal TotalPrice { get; set; } 

        [Required]
        public string OStatus { get; set; } 
    }

}