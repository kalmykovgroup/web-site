using MediatR;
using WebSite.Domain.Contracts.Dtos.SpecialOffers;

namespace WebSite.Application.CommandsAndQueries
{
    public record GetActiveSpecialOffersQuery : IRequest<List<SpecialOfferDto>>;
}
