using Reddit.APIClient;
using Reddit.HostedService;
using Reddit.Logic;
using Reddit.Models;
using Reddit.Repository;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var postChannel = Channel.CreateUnbounded<Data>();
builder.Services.AddSingleton(postChannel);
builder.Services.AddHostedService<FunnySubredditWorker>();
builder.Services.AddHostedService<PostWorker>();

builder.Services.AddHttpClient<IPostsRetrieve, PostsRetrieve>();
builder.Services.AddSingleton<IPostsProcessor, PostsProcessor>();
builder.Services.AddSingleton<IPostWithMostUpVotes , PostWithMostUpVotes>();    
builder.Services.AddSingleton<IUserWithMostPosts, UserWithMostPosts>();
builder.Services.AddSingleton<IPostRepository, PostRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}