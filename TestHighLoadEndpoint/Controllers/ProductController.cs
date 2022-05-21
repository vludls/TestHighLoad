using Microsoft.AspNetCore.Mvc;
using TestHighLoadEndpoint.Contracts.Product;
using TestHighLoadEndpoint.Services;

namespace TestHighLoadEndpoint.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly HighLoadProductService _highLoadService;

        public ProductController(HighLoadProductService highLoadService)
        {
            _highLoadService = highLoadService;
        }

        /// <summary>
        /// Получить продукт
        /// </summary>
        /// <param name="id">Идентификатор продукта</param>
        /// <returns>Продукт</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetProduct([FromRoute] int id)
        {
            var result = await _highLoadService.GetProduct(id);

            if (result is null)
                return NotFound();

            return Ok(result);
        }
    }
}