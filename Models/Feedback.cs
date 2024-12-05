using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blank.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; } // 1 to 5 stars

        public string Comment { get; set; } // Optional comment

        public string ImagePath { get; set; } // Optional image URL

        public DateTime DatePosted { get; set; } = DateTime.Now; // Thời gian đăng feedback

        public string CustomerId { get; set; } // Foreign key for the customer

        public string CustomerName { get; set; }

        public int RestaurantId { get; set; } // Foreign key for the restaurant

        [ForeignKey("RestaurantId")]
        public virtual Restaurant Restaurant { get; set; } // Navigation property to Restaurant

        public string AdminReply { get; set; } // Phản hồi của admin

        public string RStatus { get; set; } // Trạng thái của feedback (Pending, Replied, Approved, etc.)
    }
}
