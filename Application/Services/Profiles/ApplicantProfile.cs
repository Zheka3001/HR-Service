using Application.Models;
using AutoMapper;
using DataAccessLayer.Models;

namespace Application.Services.Profiles
{
    public class ApplicantProfile : Profile
    {
        public ApplicantProfile()
        {
            CreateMap<CreateApplicantRequest, ApplicantDao>()
                .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CreatorId))
                .ForMember(dest => dest.ApplicantInfo, opt => opt.MapFrom(src => new ApplicantInfoDao
                {
                    FirstName = src.FirstName,
                    LastName = src.LastName,
                    MiddleName = src.MiddleName,
                    Email = src.Email,
                    PhoneNumber = src.PhoneNumber,
                    Country = src.Country,
                    DateOfBirth = src.DateOfBirth,
                    SocialNetworks = src.SocialNetworks.Select(sn => new SocialNetworkDao
                    {
                        UserName = sn.UserName,
                        Type = (SocialNetworkTypeDao)sn.Type,
                        CreateDate = DateTime.Now,
                    }).ToList()
                }));

            CreateMap<ApplicantDao, CreateApplicantResponse>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.ApplicantInfo.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.ApplicantInfo.LastName))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.ApplicantInfo.MiddleName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.ApplicantInfo.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.ApplicantInfo.PhoneNumber))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.ApplicantInfo.Country))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.ApplicantInfo.DateOfBirth));

            CreateMap<SocialNetwork, SocialNetworkDao>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
