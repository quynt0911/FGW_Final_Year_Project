using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blank.Models
{
    public class Restaurant
    {
        [Key]
        public int RestaurantId { get; set; }

        [Required(ErrorMessage = "Restaurant name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string ResName { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(255, ErrorMessage = "Location cannot exceed 255 characters")]
        public string Location { get; set; }

        [DataType(DataType.ImageUrl)]
        public string? PhotoUrl { get; set; }

        [NotMapped]
        public IFormFile? PhotoFile { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }

        public Restaurant()
        {
            Reservations = new HashSet<Reservation>();
        }
    }
}
