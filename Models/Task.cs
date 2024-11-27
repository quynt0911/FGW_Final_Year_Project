using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Blank.Models
{
    public class Task
    {
        [Key]
        public int TaskId { get; set; }

        [Required]
        public string TaskName { get; set; }

        public string Description { get; set; }

        [Required]
        public string TaskStatus { get; set; } = "Pending";

        public string AssignedTo { get; set; } // Lưu ID của nhân viên được giao
    }
}