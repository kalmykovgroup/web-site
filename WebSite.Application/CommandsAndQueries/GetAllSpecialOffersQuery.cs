using MediatR;
using WebSite.Domain.Contracts.Dtos.SpecialOffers;

namespace WebSite.Application.CommandsAndQueries
{
    public record GetAllSpecialOffersQuery : IRequest<List<SpecialOfferDto>>;
}
