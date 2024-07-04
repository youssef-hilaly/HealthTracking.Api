using System.Diagnostics.Metrics;
using AutoMapper;
using HealthTracking.Entity.DbSet;
using HealthTracking.Entity.Dtos.Incoming;

namespace HealthTracking.Api.Configrations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<UserDto, User>().ForMember(
                dest => dest.status,
                from => from.MapFrom(x => 1)
                ).ReverseMap();

        }
    }
}
