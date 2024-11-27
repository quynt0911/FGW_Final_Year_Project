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
        public string TName { get; set; } // Table name

        [Required]
        public string Location { get; set; } // Location of the table

        [Required]
        public string TStatus { get; set; } // Table status (e.g., Available, Occupied)

        // Navigation property to Orders
        public virtual ICollection<Order> Orders { get; set; }

        // Navigation property to Reservations (if used)
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}