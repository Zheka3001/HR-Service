using Application.Models;
using AutoMapper;
using DataAccessLayer.Models;

namespace Application.Services.Profiles
{
    public class ApplicantProfile : Profile
    {
        public ApplicantProfile()
        {
            CreateMap<CreateApplicantRequest, Applicant>()
                .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CreatorId))
                .ForMember(dest => dest.ApplicantInfo, opt => opt.MapFrom(src => new ApplicantInfo
                {
                    FirstName = src.FirstName,
                    LastName = src.LastName,
                    MiddleName = src.MiddleName,
                    Email = src.Email,
                    PhoneNumber = src.PhoneNumber,
                    Country = src.Country,
                    DateOfBirth = src.DateOfBirth,
                    SocialNetworks = src.CreateSocialNetworkInfoRequests.Select(sn => new SocialNetwork
                    {
                        UserName = sn.UserName,
                        Type = sn.Type,
                        CreateDate = DateTime.Now,
                    }).ToList()
                }));

            CreateMap<Applicant, CreateApplicantResponse>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.ApplicantInfo.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.ApplicantInfo.LastName))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.ApplicantInfo.MiddleName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.ApplicantInfo.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.ApplicantInfo.PhoneNumber))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.ApplicantInfo.Country))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.ApplicantInfo.DateOfBirth));
        }
    }
}
