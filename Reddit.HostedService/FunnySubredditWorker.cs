using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
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
        private readonly IConfiguration _configuration;
        private readonly Channel<Data> _postChannel;
        private readonly IPostsRetrieve _postsRetrieve;
        private readonly string _subredditName = "funny";

        public FunnySubredditWorker(ILogger<FunnySubredditWorker> logger, IConfiguration configuration, Channel<Data> postChannel, IPostsRetrieve postsRetrieve)
        {
            _logger = logger;
            _configuration = configuration;
            _postChannel = postChannel;
            _postsRetrieve = postsRetrieve;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Funny Subreddit worker is running");

            var startingPoint = "";

            JsonSerializerOptions options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };


            while (!stoppingToken.IsCancellationRequested && await _postChannel.Writer.WaitToWriteAsync())
            {
                try
                {
                    var result = await _postsRetrieve.RetrieveSubredditPostsAsync(_subredditName, startingPoint);

                    var postData = JsonSerializer.Deserialize<Child>(result, options);

                    if(postData != null) 
                    {
                        startingPoint = postData.data.after;
                        foreach (var child in postData.data.children)
                        {
                            await _postChannel.Writer.WriteAsync(child.data);
                        }
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(600));
                }
                catch(Exception ex) 
                {
                
                }
            }
        }
    }
}