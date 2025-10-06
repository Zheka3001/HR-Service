using Application.Models;
using AutoMapper;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Profiles
{
    public class WorkGroupProfile : Profile
    {
        public WorkGroupProfile()
        {
            CreateMap<CreateWorkGroup, WorkGroupDao>();
        }
    }
}
