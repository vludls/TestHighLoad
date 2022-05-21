using System.Runtime.CompilerServices;
using TestHighLoadEndpoint.Contracts.News;
using TestHighLoadEndpoint.Helpers;
using TestHighLoadEndpoint.Repositories;

namespace TestHighLoadEndpoint.Services
{
    public class HighLoadNewsService
    {
        private readonly RequestHighLoadHelper<NewsModel> _newsModelHighLoadHelper;
        private readonly NewsRepository _newsRepository;

        public HighLoadNewsService(RequestHighLoadHelper<NewsModel> newsModelHighLoadHelper, NewsRepository newsRepository)
        {
            _newsModelHighLoadHelper = newsModelHighLoadHelper;
            _newsRepository = newsRepository;
        }

        /// <summary>
        /// Получить новость
        /// </summary>
        /// <param name="id">Идентификатор новости</param>
        /// <param name="callerMethod">Название метода, заполняется автоматически</param>
        /// <returns></returns>
        public async Task<NewsModel> GetNews(int id, [CallerMemberName] string callerMethod = "")
        {
            var key = $"Caller={callerMethod};param_id={id}";

            var result = await _newsModelHighLoadHelper.ExecuteHighLoadProcess(key, maxRequestLimit: 20000, maxExecuteTime: TimeSpan.FromSeconds(30), () =>
            {
                var news = _newsRepository.GetById(id);
                return news;
            });

            return result;
        }
    }
}
