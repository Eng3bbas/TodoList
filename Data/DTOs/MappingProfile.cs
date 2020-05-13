using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Data.Entities;

namespace TodoList.Data.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        { 
            CreateMap<User, UserDTO>()
                .ForMember(dto => dto.Identifier, m => m.MapFrom(u => u.Id));
            CreateMap<Category, CategoryDTO>()
                .ForMember(dto => dto.User, m => m.MapFrom(cat => cat.User));
            CreateMap<Todo, TodoDTO>()
                .ForMember(dto => dto.User, m => m.MapFrom(t => t.User))
                .ForMember(dto => dto.Category, m => m.MapFrom(t => t.Category));
        }
    }
}
