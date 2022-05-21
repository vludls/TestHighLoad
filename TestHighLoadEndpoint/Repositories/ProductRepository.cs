using TestHighLoadEndpoint.Contracts.Product;

namespace TestHighLoadEndpoint.Repositories
{
    public class ProductRepository
    {
        /// <summary>
        /// Хранилище продуктов
        /// </summary>
        private List<ProductModel> _productDbMemory { get; set; } = new List<ProductModel>
        {
            new ProductModel
            {
                Id = 1,
                Name = "Name1"
            },
            new ProductModel
            {
                Id = 2,
                Name = "Name2"
            }
        };

        /// <summary>
        /// Получить продукт по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Продукт</returns>
        public ProductModel GetById(int id)
        {
            var product = _productDbMemory.FirstOrDefault(product => product.Id == id);

            return product;
        }
    }
}
