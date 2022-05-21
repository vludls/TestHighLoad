using Refit;
using TestHighLoadConsole;

var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("https://localhost:7029");

var highLoadEndpointClient = RestService.For<IHighLoadEndpointClient>(httpClient);

var firstRequests = Enumerable.Range(1, 20000).Select(counter => highLoadEndpointClient.GetProduct(1)).ToArray();
var secondRequests = Enumerable.Range(1, 20000).Select(counter => highLoadEndpointClient.GetProduct(2)).ToArray();
var thirdRequests = Enumerable.Range(1, 20000).Select(counter => highLoadEndpointClient.GetNews(1)).ToArray();
var fourthRequests = Enumerable.Range(1, 20000).Select(counter => highLoadEndpointClient.GetNews(2)).ToArray();

Task.WaitAll(firstRequests);
Task.WaitAll(secondRequests);
Task.WaitAll(thirdRequests);
Task.WaitAll(fourthRequests);

Console.WriteLine("Выполнено");
