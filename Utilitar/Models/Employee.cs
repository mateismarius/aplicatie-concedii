using FluentValidation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Utilitar.Models
{

    public class Employee : BaseModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [NotMapped]
        public string? FullName { get; set; }
        public string? Location { get; set; }
        public string? PermanentLocation { get; set; }
        public bool IsDelegate { get; set; }
        public string? Marca { get; set; }
        public int DrepturiCurente { get; set; }
        public int DrepturiRestante { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime DataStart { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime DataFinal { get; set; }


        public int? RoleId { get; set; }
        public Role? Roles { get; set; }

        public ICollection<FreeDay>? FreeDays { get; set; }

        public Employee()
        {
            FullName = $"{LastName} {FirstName}";
        }

        
    }
}
