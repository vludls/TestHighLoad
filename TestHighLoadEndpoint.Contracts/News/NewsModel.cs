namespace TestHighLoadEndpoint.Contracts.News
{
    /// <summary>
    /// Новость
    /// </summary>
    public class NewsModel
    {
        /// <summary>
        /// Идентификатор новости
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Текст
        /// </summary>
        public string Text { get; set; }
    }
}
