using Application.DTOs;
using AutoMapper;
using Domain.Enums;
using Domain.Models;

namespace Application.Mappers;

public class ResultsMappingProfile : Profile
{
    public ResultsMappingProfile()
    {
        CreateMap<PlayResult, ResultResponse>()
            .ForMember(dest => dest.Result, opt => opt.MapFrom(src => Enum.Parse<GameResult>(src.Result)));
    }
}