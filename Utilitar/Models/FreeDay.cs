using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Utilitar.Models
{
    public class FreeDay : BaseModel
    {
       
        public string? RequestNumber { get; set; }

        
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime RequestDate { get; set; }

       
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime StartDate { get; set; }

        
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime FinishDate { get; set; }

        
        public int Duration { get; set; }
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public int DayOffTypeId { get; set; }
        public DayOffType? DayOffType { get; set; }

    }
}
