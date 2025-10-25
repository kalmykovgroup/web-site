using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WebSite.Application.CommandsAndQueries;
using WebSite.Domain.Contracts.Dtos;
using WebSite.Domain.Interfaces;

namespace WebSite.Application.Handlers
{

    public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllCategoriesHandler> _logger;

        public GetAllCategoriesHandler(
            ICategoryRepository categoryRepository,
            IMapper mapper,
            ILogger<GetAllCategoriesHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<CategoryDto>> Handle(
            GetAllCategoriesQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Получение всех категорий");

            try
            {
                var categories = await _categoryRepository.GetAllAsync(cancellationToken);

                // Используем AutoMapper для маппинга списка
                var categoriesDto = _mapper.Map<List<CategoryDto>>(categories);

                _logger.LogInformation("Успешно получено {Count} категорий", categoriesDto.Count);

                return categoriesDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении всех категорий");
                throw;
            }
        }
    }

}