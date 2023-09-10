using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reddit.APIClient;
using Reddit.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;

namespace Reddit.HostedService
{
    public class FunnySubredditWorker : BackgroundService
    {
        private readonly ILogger<FunnySubredditWorker> _logger;        
        private readonly Channel<Data> _postChannel;
        private readonly IPostsService _postsRetrieve;
        private readonly string _subredditName = "funny";
        private readonly int _delayStart = 5;

        public FunnySubredditWorker(ILogger<FunnySubredditWorker> logger, Channel<Data> postChannel, IPostsService postsRetrieve, IConfiguration configuration)
        {
            _logger = logger;            
            _postChannel = postChannel;
            _postsRetrieve = postsRetrieve;
            var delayStart = configuration["DelayStart"];

            if (!int.TryParse(delayStart, out _delayStart))
            {
                logger.LogWarning("Funny Subreddit Worker : Can not get delay start value default back to 5 ");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Funny Subreddit worker is running");

            var startingPoint = "";

            JsonSerializerOptions options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            await Task.Delay(TimeSpan.FromSeconds(_delayStart), stoppingToken);

            while (!stoppingToken.IsCancellationRequested && await _postChannel.Writer.WaitToWriteAsync(stoppingToken))
            {
                try
                {
                    _logger.LogInformation("Funny Subreddit Worker: Start getting posts");
                    var result = await _postsRetrieve.RetrieveSubredditPostsAsync(_subredditName, startingPoint);

                    var postData = JsonSerializer.Deserialize<Child>(result, options);

                    if (postData != null)
                    {
                        _logger.LogInformation($"Funny Subreddit Worker: Retrieve {postData?.data?.children?.Count}");
                        if(postData == null) 
                        {
                            throw new Exception("Funny Subreddit Worker: Post data is null");
                        }
                        startingPoint = postData.data.after;
                        foreach (var child in postData.data.children)
                        {
                            await _postChannel.Writer.WriteAsync(child.data, stoppingToken);
                        }
                    }
                    else
                    {
                        throw new Exception("Funny Subreddit Worker: post data is null");
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(600), stoppingToken);
                }
                catch(Exception ex) 
                {
                    _logger.LogError(ex, "Funny Subreddit Worker: Failed to retrieve data from Reddit");
                    throw;
                }
            }            
            _postChannel.Writer.Complete();
        }
    }
}