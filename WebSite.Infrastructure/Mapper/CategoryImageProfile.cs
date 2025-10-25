using AutoMapper;
using WebSite.Domain.Contracts.Dtos;
using WebSite.Domain.Models;

namespace WebSite.Infrastructure.Mapper
{ 
    public class CategoryImageProfile : Profile
    {
        public CategoryImageProfile()
        {
            // Обратный маппинг (если нужно)
            CreateMap<CategoryImageDto, CategoryImage>().ReverseMap();

        }
    }
}
