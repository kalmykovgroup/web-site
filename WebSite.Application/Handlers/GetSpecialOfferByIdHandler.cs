using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WebSite.Application.CommandsAndQueries;
using WebSite.Domain.Contracts.Dtos.SpecialOffers;
using WebSite.Domain.Interfaces;

namespace WebSite.Application.Handlers
{
    public class GetSpecialOfferByIdHandler : IRequestHandler<GetSpecialOfferByIdQuery, SpecialOfferDto?>
    {
        private readonly ISpecialOffersRepository _specialOffersRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetSpecialOfferByIdHandler> _logger;

        public GetSpecialOfferByIdHandler(
            ISpecialOffersRepository specialOffersRepository,
            IMapper mapper,
            ILogger<GetSpecialOfferByIdHandler> logger)
        {
            _specialOffersRepository = specialOffersRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SpecialOfferDto?> Handle(
            GetSpecialOfferByIdQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Получение специального предложения по ID: {Id}", request.Id);

            try
            {
                var offer = await _specialOffersRepository.GetByIdAsync(request.Id, cancellationToken);

                if (offer == null)
                {
                    _logger.LogWarning("Специальное предложение с ID {Id} не найдено", request.Id);
                    return null;
                }

                var offerDto = _mapper.Map<SpecialOfferDto>(offer);

                _logger.LogInformation("Успешно получено специальное предложение с ID: {Id}", request.Id);

                return offerDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении специального предложения по ID: {Id}", request.Id);
                throw;
            }
        }
    }
}
