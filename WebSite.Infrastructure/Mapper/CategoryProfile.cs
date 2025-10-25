using AutoMapper;
using WebSite.Domain.Contracts.Dtos;
using WebSite.Domain.Models;

namespace WebSite.Infrastructure.Mapper
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        { 
            // Обратный маппинг (если нужно)
            CreateMap<CategoryDto, Category>().ReverseMap();
          
        }
    }
}
