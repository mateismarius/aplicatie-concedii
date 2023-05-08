using System.ComponentModel.DataAnnotations;

namespace Utilitar.Models
{
    public class LegallDay : BaseModel
    {
        [Required(ErrorMessage = "* Acest camp este obligatoriu")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "* Acest camp este obligatoriu")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateOff { get; set; }
    }
}
