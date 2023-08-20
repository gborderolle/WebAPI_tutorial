using WebAPI_tutorial.Models;
using WebAPI_tutorial.Models.DTOs;
using AutoMapper;

namespace WebAPI_tutorial.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Author, AuthorDTO>().ReverseMap();
            CreateMap<Author, AuthorCreateDTO>().ReverseMap();
            CreateMap<Author, AuthorUpdateDTO>().ReverseMap();

            CreateMap<Book, BookDTO>().ReverseMap();
            CreateMap<Book, BookCreateDto>().ReverseMap();
            CreateMap<Book, BookUpdateDTO>().ReverseMap();
        }
    }
}
