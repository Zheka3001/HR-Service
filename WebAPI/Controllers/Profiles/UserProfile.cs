using Application.Models;
using AutoMapper;
using WebAPI.DTOs;

namespace WebAPI.Controllers.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterUserDto, RegisterUser>();
            CreateMap<LoginRequestDto, LoginRequest>();
        }
    }
}
