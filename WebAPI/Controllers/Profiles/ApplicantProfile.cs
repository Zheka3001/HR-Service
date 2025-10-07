using Application.Models;
using AutoMapper;
using DataAccessLayer.Models;
using Model.Search;
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

            CreateMap<ApplicantSearchResult, ApplicantSearchResultDto>();

            CreateMap<QueryResultByCriteria<ApplicantSearchResult>, QueryResultByCriteria<ApplicantSearchResultDto>>();
        }
    }
}
