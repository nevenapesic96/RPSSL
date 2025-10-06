using Application.DTOs;
using AutoMapper;
using Domain.Enums;

namespace Application.Mappers;

public class ChoiceMappingProfile : Profile
{
    public ChoiceMappingProfile()
    {
        CreateMap<Choices, ChoiceResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (int)src))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ToString()));
    }
}