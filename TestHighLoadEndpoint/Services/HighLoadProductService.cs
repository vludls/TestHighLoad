using System.Runtime.CompilerServices;
using TestHighLoadEndpoint.Contracts.Product;
using TestHighLoadEndpoint.Helpers;
using TestHighLoadEndpoint.Repositories;

namespace TestHighLoadEndpoint.Services
{
    public class HighLoadProductService
    {
        private readonly RequestHighLoadHelper<ProductModel> _productModelHighLoadHelper;
        private readonly ProductRepository _productRepository;

        public HighLoadProductService(RequestHighLoadHelper<ProductModel> productModelHighLoadHelper, ProductRepository productRepository)
        {
            _productModelHighLoadHelper = productModelHighLoadHelper;
            _productRepository = productRepository;
        }

        public async Task<ProductModel> GetProduct(int id, [CallerMemberName] string callerMethod = "")
        {
            var key = $"Caller={callerMethod};param_id={id}";

            var result = await _productModelHighLoadHelper.ExecuteHighLoadProcess(key, maxRequestLimit: 20000, maxExecuteTime: TimeSpan.FromSeconds(30), () =>
            {
                var product = _productRepository.GetById(id);
                return product;
            });

            return result;
        }
    }
}
