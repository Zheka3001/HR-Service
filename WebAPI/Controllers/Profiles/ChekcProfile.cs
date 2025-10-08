using Application.Models;
using AutoMapper;
using WebAPI.DTOs;

namespace WebAPI.Controllers.Profiles
{
    public class ChekcProfile : Profile
    {
        public ChekcProfile()
        {
            CreateMap<CheckResult, CheckResultDto>();
        }
    }
}
