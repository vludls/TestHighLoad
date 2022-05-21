using System.Collections.Concurrent;
using TestHighLoadEndpoint.Services;
using Timer = System.Timers.Timer;

namespace TestHighLoadEndpoint.Helpers
{
    /// <summary>
    /// Позволяет выполнять высоконагруженные операции
    /// </summary>
    public class RequestHighLoadHelper<TResult>
    {
        private readonly RequestObserver _requestObserver;
        private readonly TimerObserver _timerObserver;
        private readonly TaskObserver _taskObserver;

        private readonly ILogger<RequestHighLoadHelper<TResult>> _logger;

        public RequestHighLoadHelper(ILogger<RequestHighLoadHelper<TResult>> logger)
        {
            _requestObserver = new RequestObserver();
            _taskObserver = new TaskObserver();
            _timerObserver = new TimerObserver();
            _logger = logger;
        }

        /// <summary>
        /// Выполнить высоконагруженную операцию
        /// </summary>
        /// <typeparam name="TResult">Тип результата, возвращаемый функцией</typeparam>
        /// <param name="key">Ключ запроса</param>
        /// <param name="maxExecuteTime">Максимальное время выполнения запроса</param>
        /// <param name="process">Что должно выполниться, когда лимит запросов будет максимальный/время выполнения запроса истечёт</param>
        /// <returns></returns>
        public async Task<TResult> ExecuteHighLoadProcess(string key, long maxRequestLimit, TimeSpan maxExecuteTime, Func<TResult> process)
        {
            _taskObserver.AddTask(key, _logger);
            var taskCompletionSource = _taskObserver[key];

            _requestObserver.AddRequest(key, maxRequestLimit, _logger);
            _requestObserver.RequestLimitElapsed += ExecuteQueries;

            _timerObserver.AddTimerForRequests(key, maxExecuteTime, ExecuteQueries, _logger);

            return await taskCompletionSource.Task;

            /// <summary>
            /// Выполнить запросы по ключу
            /// </summary>
            void ExecuteQueries(object sender, ExecuteRequestArgs args)
            {
                if (args.Key == key)
                {
                    if (taskCompletionSource.Task.IsCompletedSuccessfully)
                        return;

                    var result = process();
                    taskCompletionSource.TrySetResult(result);
                    _taskObserver.TryRemove(key, out _);
                }
            }
        }

        /// <summary>
        /// Наблюдает за поступившими таймерами по ключу запроса
        /// </summary>
        public class TimerObserver : ConcurrentDictionary<string, Timer>
        {
            private object lockObject = new object();

            /// <summary>
            /// Добавить один таймер для всех запросов по ключу
            /// </summary>
            /// <param name="key">Ключ запроса</param>
            /// <param name="maxExecuteTime">Максимальное время выполнения</param>
            /// <param name="action">Что нужно сделать по истечению таймера?</param>
            public void AddTimerForRequests(string key, TimeSpan maxExecuteTime, Action<object, ExecuteRequestArgs> action, ILogger<RequestHighLoadHelper<TResult>> logger)
            {
                lock (lockObject)
                {
                    if (!ContainsKey(key))
                    {
                        var timer = new Timer();
                        this[key] = timer;

                        timer.Elapsed += (sender, args) =>
                        {
                            logger.LogInformation($"Время выполнения запросов с ключём {key} истекло, запускается принудительное выполнение");
                            action(sender, new ExecuteRequestArgs { Key = key });
                            timer.Stop();
                            timer.Close();
                            this.Remove(key, out _);
                        };

                        timer.Interval = maxExecuteTime.TotalMilliseconds;
                        timer.Start();
                    }
                }
            }
        }

        /// <summary>
        /// Наблюдает за поступившими тасками по ключу запроса
        /// </summary>
        public class TaskObserver : ConcurrentDictionary<string, TaskCompletionSource<TResult>>
        {
            private object lockObject = new object();

            /// <summary>
            /// Добавить одну задачу для всех запросов по ключу
            /// </summary>
            /// <param name="key">Ключ запроса</param>
            public void AddTask(string key, ILogger<RequestHighLoadHelper<TResult>> logger)
            {
                lock (lockObject)
                {
                    if (!ContainsKey(key))
                    {
                        logger.LogInformation($"Добавлена одна задача для запросов с ключём {key}");
                        this[key] = new TaskCompletionSource<TResult>();
                    }
                }
            }
        }

        /// <summary>
        /// Наблюдает за поступившими запросами по ключу запроса
        /// </summary>
        public class RequestObserver : ConcurrentDictionary<string, long>
        {
            private object lockObject = new object();

            /// <summary>
            /// Событие вызывается тогда, когда достигнут максимальный лимит запросов по одному ключу
            /// </summary>
            public event Action<object, ExecuteRequestArgs> RequestLimitElapsed;

            /// <summary>
            /// Добавить запрос в счётчик по ключу
            /// </summary>
            /// <param name="key">Ключ запроса</param>
            public void AddRequest(string key, long maxRequestLimit, ILogger<RequestHighLoadHelper<TResult>> logger)
            {
                lock (lockObject)
                {
                    if (ContainsKey(key))
                    {
                        this[key] += 1;
                    }
                    else
                    {
                        TryAdd(key, 1);
                    }

                    logger.LogInformation($"Для ключа {key}, кол-во запросов стало {this[key]}");

                    if (base[key] == maxRequestLimit)
                    {
                        logger.LogInformation($"Для ключа {key} достигнут лимит в {maxRequestLimit}, начинается выполнение");
                        CallRequestLimitElapsedEvent(key);
                    }
                }
            }

            /// <summary>
            /// Вызвать событие: Достигнут максимальный лимит запросов
            /// </summary>
            /// <param name="key">Ключ запроса</param>
            public void CallRequestLimitElapsedEvent(string key)
            {
                base[key] = 0;
                RequestLimitElapsed(this, new ExecuteRequestArgs { Key = key });
            }
        }

        public class ExecuteRequestArgs
        {
            /// <summary>
            /// Ключ запроса
            /// </summary>
            public string Key { get; set; }
        }
    }
}
