
using AutoMapper;
using Newtonsoft.Json;
using Valid.Teste.API.Models;
using Valid.Teste.Domain.Entities;
using Profile = AutoMapper.Profile;

namespace Valid.Teste.API.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Domain.Entities.Profile, ProfileParameter>()
            .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.ProfileName))
            .ForMember(dest => dest.Parameters, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<Dictionary<string, string>>(src.Parameters))) 
            .ReverseMap()
            .ForMember(dest => dest.Parameters, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Parameters)));



        }
    }
}
