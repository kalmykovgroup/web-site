using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WebSite.Application.CommandsAndQueries;
using WebSite.Domain.Contracts.Dtos.SpecialOffers;
using WebSite.Domain.Interfaces;

namespace WebSite.Application.Handlers
{
    public class GetActiveSpecialOffersHandler : IRequestHandler<GetActiveSpecialOffersQuery, List<SpecialOfferDto>>
    {
        private readonly ISpecialOffersRepository _specialOffersRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetActiveSpecialOffersHandler> _logger;

        public GetActiveSpecialOffersHandler(
            ISpecialOffersRepository specialOffersRepository,
            IMapper mapper,
            ILogger<GetActiveSpecialOffersHandler> logger)
        {
            _specialOffersRepository = specialOffersRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<SpecialOfferDto>> Handle(
            GetActiveSpecialOffersQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Получение активных специальных предложений");

            try
            {
                var offers = await _specialOffersRepository.GetActiveAsync(cancellationToken);

                var offersDto = _mapper.Map<List<SpecialOfferDto>>(offers);

                _logger.LogInformation("Успешно получено {Count} активных специальных предложений", offersDto.Count);

                return offersDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении активных специальных предложений");
                throw;
            }
        }
    }
}
