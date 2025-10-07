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

            CreateMap<SocialNetwork, SocialNetworkDao>()
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
