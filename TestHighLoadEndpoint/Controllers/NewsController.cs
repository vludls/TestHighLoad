using Microsoft.AspNetCore.Mvc;
using TestHighLoadEndpoint.Contracts.News;
using TestHighLoadEndpoint.Services;

namespace TestHighLoadEndpoint.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly HighLoadNewsService _highLoadNewsService;

        public NewsController(HighLoadNewsService highLoadNewsService)
        {
            _highLoadNewsService = highLoadNewsService;
        }

        /// <summary>
        /// Получить новость
        /// </summary>
        /// <param name="id">Идентификатор новости</param>
        /// <returns>Новость</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<NewsModel>> GetNews([FromRoute] int id)
        {
            var result = await _highLoadNewsService.GetNews(id);

            if (result is null)
                return NotFound();

            return Ok(result);
        }
    }
}
