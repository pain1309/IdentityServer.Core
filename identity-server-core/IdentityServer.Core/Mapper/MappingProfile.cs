using AutoMapper;
using IdentityServer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Core.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterViewModel, ApplicationUser>()
            .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
        }
    }
}
