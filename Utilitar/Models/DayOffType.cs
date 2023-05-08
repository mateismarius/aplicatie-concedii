using System.ComponentModel.DataAnnotations;

namespace Utilitar.Models
{
    public class DayOffType : BaseModel
    {
        [Required(ErrorMessage = "* Acest camp este obligatoriu")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "* Acest camp este obligatoriu")]
        public string? Description { get; set; }
        public bool IsPaid { get; set; }

        public ICollection<FreeDay>? FreeDays { get; set; }
    }
}
