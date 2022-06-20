using AutoMapper;
using Monovi.Model.Concrete;
using Monovi.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Monovi.Web.UI.Helpers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserViewModel>().ReverseMap();
        }
    }
}
