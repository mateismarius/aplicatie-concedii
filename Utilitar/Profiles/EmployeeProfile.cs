using AutoMapper;
using Utilitar.Models;
using Utilitar.ViewModels;

namespace Utilitar.Profiles
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeVM>()
                .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Id)
                )
                .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => $"{src.LastName + " " + src.FirstName}")
                )
                .ForMember(
                dest => dest.Roles,
                opt => opt.MapFrom(src => $"{src.Roles.RoleName}")
                )
                .ForMember(
                dest => dest.Location,
                opt => opt.MapFrom(src => $"{src.Location}")
                )
                .ForMember(
                dest => dest.PermanentLocation,
                opt => opt.MapFrom(src => $"{src.PermanentLocation}")
                )
                .ForMember(
                dest => dest.IsDelegate,
                opt => opt.MapFrom(src => src.IsDelegate.Equals(true) ? "Delegat" : "Titular")
                )
                .ForMember(
                dest => dest.Marca,
                opt => opt.MapFrom(src => $"{src.Marca}")
                )
                .ForMember(
                dest => dest.DataStart,
                opt => opt.MapFrom(src => $"{src.DataStart.ToShortDateString()}")
                )
                .ForMember(
                dest => dest.DataFinal,
                opt => opt.MapFrom(src => $"{src.DataFinal.ToShortDateString()}")
                )
                .ForMember(
                dest => dest.DrepturiCurente,
                opt => opt.MapFrom(src => $"{src.DrepturiCurente}"))
                .ForMember(
                dest => dest.DrepturiRestante,
                opt => opt.MapFrom(src => $"{src.DrepturiRestante}"));

        }
    
    }
}
