using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
public class Report
{
    [Key]
    public string ReportId { get; set;}
    public string Title { get; set; }
    public decimal Value { get; set; }
    public DateTime Date { get; set; }
}
