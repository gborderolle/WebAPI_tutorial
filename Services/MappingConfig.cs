using WebAPI_tutorial.Models;
using WebAPI_tutorial.Models.Dto;
using AutoMapper;

namespace WebAPI_tutorial.Services
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Author, AuthorDto>().ReverseMap();
            CreateMap<Author, AuthorCreateDto>().ReverseMap();
            CreateMap<Author, AuthorUpdateDto>().ReverseMap();

            CreateMap<Book, BookDto>().ReverseMap();
            CreateMap<Book, BookCreateDto>().ReverseMap();
            CreateMap<Book, BookUpdateDto>().ReverseMap();
        }
    }
}
