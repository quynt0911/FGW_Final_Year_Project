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
        public string TName { get; set; }
        public string Location { get; set; }
        public string TStatus { get; set; }
    }
}