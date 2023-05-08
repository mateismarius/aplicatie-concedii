using System.ComponentModel.DataAnnotations;

namespace Utilitar.Models
{
    public class Role
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "* Acest camp este obligatoriu")]
        public string? RoleName{ get; set; }
        [Required(ErrorMessage = "* Acest camp este obligatoriu")]
        public int PriorityNo { get; set; }
    }
}
