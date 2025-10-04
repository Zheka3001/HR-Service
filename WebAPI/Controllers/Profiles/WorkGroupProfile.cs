using Application.Models;
using AutoMapper;
using WebAPI.DTOs;

namespace WebAPI.Controllers.Profiles
{
    public class WorkGroupProfile : Profile
    {
        public WorkGroupProfile()
        {
            CreateMap<CreateWorkGroupDto, CreateWorkGroup>();
        }
    }
}
