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
builder.Services.AddHostedService<SubredditHostedService>();
builder.Services.AddHostedService<PostHostedService>();

builder.Services.AddHttpClient<IPostsService, PostsService>();
builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>();
builder.Services.AddSingleton<IPostsProcessor, PostsProcessor>();
builder.Services.AddSingleton<IPostWithMostUpVotes , PostWithMostUpVotes>();    
builder.Services.AddSingleton<IUserWithMostPosts, UserWithMostPosts>();
builder.Services.AddSingleton<IPostRepository, PostRepository>();
builder.Services.AddSingleton<IRateLimitChecker, RateLimitChecker>();
builder.Services.AddSingleton<ISubredditService, SubredditService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/Top10UserWithMostPosts", (IUserWithMostPosts post) =>
{

    return post.Retrieve();


});

app.MapGet("/Top10PostWithMostUpVotes", (IPostWithMostUpVotes post) =>
{

    return post.Retrieve();


});

app.Run();