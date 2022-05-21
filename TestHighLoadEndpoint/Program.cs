using TestHighLoadEndpoint.Contracts.News;
using TestHighLoadEndpoint.Contracts.Product;
using TestHighLoadEndpoint.Helpers;
using TestHighLoadEndpoint.Repositories;
using TestHighLoadEndpoint.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddLogging();
builder.Services.AddSingleton<ProductRepository>();
builder.Services.AddSingleton<NewsRepository>();
builder.Services.AddSingleton<RequestHighLoadHelper<ProductModel>>();
builder.Services.AddSingleton<RequestHighLoadHelper<NewsModel>>();
builder.Services.AddSingleton<HighLoadProductService>();
builder.Services.AddSingleton<HighLoadNewsService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
