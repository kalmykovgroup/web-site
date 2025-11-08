using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebSite.Application.CommandsAndQueries;
using WebSite.Domain.Contracts.Dtos.SpecialOffers;

namespace WebSite.Api.Controllers
{
    [ApiController]
    [Route("api/special-offers")]
    [Produces("application/json")]
    public class SpecialOffersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SpecialOffersController> _logger;

        public SpecialOffersController(
            IMediator mediator,
            ILogger<SpecialOffersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Получить все специальные предложения (включая неактивные)
        /// </summary>
        /// <returns>Список всех специальных предложений</returns>
        /// <response code="200">Успешное получение списка</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<SpecialOfferDto>), 200)]
        public async Task<ActionResult<IEnumerable<SpecialOfferDto>>> GetAllOffers(CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetAllSpecialOffersQuery();
                var result = await _mediator.Send(query, cancellationToken);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении всех специальных предложений");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "Внутренняя ошибка сервера" }
                );
            }
        }

        /// <summary>
        /// Получить только активные специальные предложения
        /// </summary>
        /// <returns>Список активных специальных предложений</returns>
        /// <response code="200">Успешное получение списка</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<SpecialOfferDto>), 200)]
        public async Task<ActionResult<IEnumerable<SpecialOfferDto>>> GetActiveOffers(CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetActiveSpecialOffersQuery();
                var result = await _mediator.Send(query, cancellationToken);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении активных предложений");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "Внутренняя ошибка сервера" }
                );
            }
        }

        /// <summary>
        /// Получить специальное предложение по ID
        /// </summary>
        /// <param name="id">ID предложения</param>
        /// <returns>Специальное предложение</returns>
        /// <response code="200">Предложение найдено</response>
        /// <response code="404">Предложение не найдено</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(SpecialOfferDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<SpecialOfferDto>> GetOfferById(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetSpecialOfferByIdQuery(id);
                var result = await _mediator.Send(query, cancellationToken);

                if (result == null)
                {
                    return NotFound(new { message = $"Специальное предложение с ID {id} не найдено" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении предложения");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "Внутренняя ошибка сервера" }
                );
            }
        }
    }
}
