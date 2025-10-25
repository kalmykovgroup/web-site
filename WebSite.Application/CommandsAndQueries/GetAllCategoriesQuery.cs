using MediatR;
using WebSite.Domain.Contracts.Dtos;

namespace WebSite.Application.CommandsAndQueries
{
    public record GetAllCategoriesQuery : IRequest<List<CategoryDto>>;
}
