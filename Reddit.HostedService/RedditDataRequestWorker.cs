using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reddit.Models;
using System.Threading.Channels;

namespace Reddit.HostedService
{
    public class RedditDataRequestWorker : BackgroundService
    {
        private readonly ILogger<RedditDataRequestWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly Channel<Post> _postChannel;

        public RedditDataRequestWorker(ILogger<RedditDataRequestWorker> logger, IConfiguration configuration, Channel<Post> postChannel)
        {
            _logger = logger;
            _configuration = configuration;
            _postChannel = postChannel;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Hosted Service is running");

            await _postChannel.Writer.WriteAsync(new Post());
        }
    }
}