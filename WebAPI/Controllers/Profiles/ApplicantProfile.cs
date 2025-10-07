using Application.Models;
using AutoMapper;
using WebAPI.DTOs;

namespace WebAPI.Controllers.Profiles
{
    public class ApplicantProfile : Profile
    {
        public ApplicantProfile()
        {
            CreateMap<CreateApplicantRequestDto, CreateApplicantRequest>();
            CreateMap<SocialNetworkDto, SocialNetwork>();

            CreateMap<UpdateApplicantRequestDto, UpdateApplicantRequest>();
        }
    }
}
