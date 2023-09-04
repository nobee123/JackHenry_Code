using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reddit.APIClient;
using Reddit.Models;
using System.Threading.Channels;

namespace Reddit.HostedService
{
    public class RedditDataRequestWorker : BackgroundService
    {
        private readonly ILogger<RedditDataRequestWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly Channel<Post> _postChannel;
        private readonly IRedditClient _redditClient;

        public RedditDataRequestWorker(ILogger<RedditDataRequestWorker> logger, IConfiguration configuration, Channel<Post> postChannel, IRedditClient redditClient)
        {
            _logger = logger;
            _configuration = configuration;
            _postChannel = postChannel;
            _redditClient = redditClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Hosted Service is running");

            while (!stoppingToken.IsCancellationRequested && await _postChannel.Writer.WaitToWriteAsync())
            {
                await _postChannel.Writer.WriteAsync(new Post());

                await Task.Delay(TimeSpan.FromMilliseconds(10));
            }
        }
    }
}