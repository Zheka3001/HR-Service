using Application.Models;
using AutoMapper;
using DataAccessLayer.Models;
using WebAPI.DTOs;

namespace WebAPI.Controllers.Profiles
{
    public class ApplicantProfile : Profile
    {
        public ApplicantProfile()
        {
            CreateMap<CreateApplicantRequestDto, CreateApplicantRequest>()
                .ForMember(dest => dest.CreateSocialNetworkInfoRequests, opt => opt.MapFrom(src => src.CreateSocialNetworkRequestList));
            CreateMap<CreateSocialNetworkInfoRequestDto, CreateSocialNetworkInfoRequest>();

            CreateMap<CreateApplicantResponse, CreateApplicantResponseDto>();
        }
    }
}
