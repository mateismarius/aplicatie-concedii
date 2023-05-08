using AutoMapper;
using Utilitar.Models;
using Utilitar.ViewModels;

namespace Utilitar.Profiles
{
    public class FreeDayProfile : Profile
    {
        public FreeDayProfile()
        {
            CreateMap<FreeDay, FreeDayVM>()
                .ForMember(
                dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.Employee.LastName + " " + src.Employee.FirstName}")
                )
                .ForMember(
                dest => dest.RequestDate,
                opt => opt.MapFrom(src => $"{src.RequestDate.ToShortDateString()}")
                )
                .ForMember(
                dest => dest.StartDate,
                opt => opt.MapFrom(src => $"{src.StartDate.ToShortDateString()}")
                )
                .ForMember(
                dest => dest.FinishDate,
                opt => opt.MapFrom(src => $"{src.FinishDate.ToShortDateString()}")
                );
        }
                
    }
}
