using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebSite.Application.CommandsAndQueries;
using WebSite.Domain.Contracts.Dtos;

namespace WebSite.Api.Controllers
{
    [ApiController]
    [Route("api/categories")]
    [Produces("application/json")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(
            IMediator mediator,
            ILogger<CategoryController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Получить все категории
        /// </summary>
        /// <returns>Список всех категорий</returns>
        [HttpGet]
        [Route("all")] 
        
        public async Task<ActionResult<List<CategoryDto>>> GetAllCategories(CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetAllCategoriesQuery();
                var result = await _mediator.Send(query, cancellationToken);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка категорий");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "Внутренняя ошибка сервера" }
                );
            }
        }

      
    }
}

