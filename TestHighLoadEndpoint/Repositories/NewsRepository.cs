using TestHighLoadEndpoint.Contracts.News;

namespace TestHighLoadEndpoint.Repositories
{
    public class NewsRepository
    {
        /// <summary>
        /// Хранилище продуктов
        /// </summary>
        private List<NewsModel> _productDbMemory { get; set; } = new List<NewsModel>
        {
            new NewsModel
            {
                Id = 1,
                Text = "Текст новости 1"
            },
            new NewsModel
            {
                Id = 2,
                Text = "Текст новости 2"
            }
        };

        /// <summary>
        /// Получить продукт по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Продукт</returns>
        public NewsModel GetById(int id)
        {
            var product = _productDbMemory.FirstOrDefault(product => product.Id == id);

            return product;
        }
    }
}
