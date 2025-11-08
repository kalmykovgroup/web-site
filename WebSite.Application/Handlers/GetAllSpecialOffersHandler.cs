using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WebSite.Application.CommandsAndQueries;
using WebSite.Domain.Contracts.Dtos.SpecialOffers;
using WebSite.Domain.Interfaces;

namespace WebSite.Application.Handlers
{
    public class GetAllSpecialOffersHandler : IRequestHandler<GetAllSpecialOffersQuery, List<SpecialOfferDto>>
    {
        private readonly ISpecialOffersRepository _specialOffersRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllSpecialOffersHandler> _logger;

        public GetAllSpecialOffersHandler(
            ISpecialOffersRepository specialOffersRepository,
            IMapper mapper,
            ILogger<GetAllSpecialOffersHandler> logger)
        {
            _specialOffersRepository = specialOffersRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<SpecialOfferDto>> Handle(
            GetAllSpecialOffersQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Получение всех специальных предложений");

            try
            {
                var offers = await _specialOffersRepository.GetAllAsync(cancellationToken);

                var offersDto = _mapper.Map<List<SpecialOfferDto>>(offers);

                _logger.LogInformation("Успешно получено {Count} специальных предложений", offersDto.Count);

                return offersDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении всех специальных предложений");
                throw;
            }
        }
    }
}
