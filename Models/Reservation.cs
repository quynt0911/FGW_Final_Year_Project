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
        public int? TableId { get; set; }

        [ForeignKey("Restaurant")]
        public int RestaurantId { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [MaxLength(50)]
        public string RStatus { get; set; } = "Pending";

        [MaxLength(500)]
        public string Request { get; set; } 

        [MaxLength(100)]
        public string CustomerName { get; set; } 

        [Required(AllowEmptyStrings = true)]
        public string  CustomerId{ get; set; }

        public virtual Table Table { get; set; } 
        public virtual Restaurant Restaurant { get; set; } 

    }
}
