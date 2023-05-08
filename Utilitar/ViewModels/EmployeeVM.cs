using System.ComponentModel.DataAnnotations;
using Utilitar.Models;

namespace Utilitar.ViewModels
{
    public class EmployeeVM  
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Roles { get; set; }
        public string? Location { get; set; }
        public string? PermanentLocation { get; set; }
        public string? IsDelegate { get; set; }
        public string? Marca { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public string? DataStart { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public string? DataFinal { get; set; }
        public int DrepturiCurente { get; set; }
        public int DrepturiRestante { get; set; }
        public int Efectuat { get; set; }
        public int DaysLeft { get; set ; }
        public int CSEfectuat { get; set; }

    }
}
