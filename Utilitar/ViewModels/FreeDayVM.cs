using System.ComponentModel.DataAnnotations;
using Utilitar.Models;

namespace Utilitar.ViewModels
{
    public class FreeDayVM : FreeDay
    {
        public string? FullName { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public new string? RequestDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public new string? StartDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public new string? FinishDate { get; set; }
    }
}
