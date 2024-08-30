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
        public string TaskName { get; set; }
        public string Description { get; set; }
        public string TaskStatus { get; set; }
    }
}