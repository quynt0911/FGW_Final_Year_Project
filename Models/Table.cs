using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Blank.Models
{
    public class Table
    {
        [Key]
        public int TableId { get; set; }

        [Required]
        public string TName { get; set; }

        // [Required]
        public string Location { get; set; } 

        // [Required]
        public string TStatus { get; set; } 

        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}