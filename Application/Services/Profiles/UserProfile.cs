using Application.Models;
using AutoMapper;
using DataAccessLayer.Models;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile() 
        {
            var hmac = new HMACSHA512();
            CreateMap<RegisterUser, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => 
                    hmac.ComputeHash(Encoding.UTF8.GetBytes(src.Password))))
                .ForMember(dest => dest.PasswordSalt, opt => opt.MapFrom(src => hmac.Key))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Role.HR));
        }
    }
}
