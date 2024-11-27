using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blank.Models
{
public class Dish
{
    [Key]
    public int DishId { get; set; }

    [Required(ErrorMessage = "Dish name is required.")]
    public string DishName { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    public string DDescription { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }
    public virtual ICollection<Order> Orders { get; set; }

    [DataType(DataType.ImageUrl)]
    public string? PhotoUrl { get; set; }

    [NotMapped]
    public IFormFile? PhotoFile { get; set; }
}
}