using Refit;
using TestHighLoadEndpoint.Contracts.News;
using TestHighLoadEndpoint.Contracts.Product;

namespace TestHighLoadConsole
{
    public interface IHighLoadEndpointClient
    {
        /// <summary>
        /// Получить продукт
        /// </summary>
        /// <param name="id">Идентификатор продукта</param>
        /// <returns>Продукт</returns>
        [Get("/Product/{id}")]
        Task<ProductModel> GetProduct(int id);

        /// <summary>
        /// Получить новость
        /// </summary>
        /// <param name="id">Идентификатор новости</param>
        /// <returns>Новость</returns>
        [Get("/News/{id}")]
        Task<NewsModel> GetNews(int id);
    }
}
