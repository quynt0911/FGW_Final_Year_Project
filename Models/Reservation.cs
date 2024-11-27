using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blank.Models
{
    public class Reservation
    {
        [Key]
        public int ReserId { get; set; }

        [ForeignKey("Table")]
        public int? TableId { get; set; } // Nullable for "Quick Pick" option

        [ForeignKey("Restaurant")]
        public int RestaurantId { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [MaxLength(50)]
        public string RStatus { get; set; } // Pending, Approved, Rejected

        [MaxLength(500)]
        public string Request { get; set; } // Optional special requests

        [MaxLength(20)]
        public string TableSelectionMethod { get; set; } // Quick Pick or Select Table

        [MaxLength(100)]
        public string CustomerName { get; set; } // Name of the customer

        // Navigation Properties
        public virtual Table Table { get; set; } // Related table
        public virtual Restaurant Restaurant { get; set; } // Related restaurant
        public string TableOption { get; set; }

    }
}
