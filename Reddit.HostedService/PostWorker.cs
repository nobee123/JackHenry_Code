using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reddit.Logic;
using Reddit.Models;
using System.Threading.Channels;

namespace Reddit.HostedService
{
    public class PostWorker : BackgroundService
    {
        private readonly Channel<Data> _postChannel;
        private readonly IPostsProcessor _postProcessor;
        private readonly int _numberOfWorker;
        private readonly ILogger _logger;
        private readonly int _delayStart;

        public PostWorker(Channel<Data> postChannel, IPostsProcessor postsProcessor, IConfiguration configuration, ILogger<PostWorker> logger)
        {
            _postChannel = postChannel;
            _postProcessor = postsProcessor;
            _logger = logger;

            var numberOfWorker = Helper.Helper.GetConfigValue(configuration, "NumberOfWorker");

            if (!int.TryParse(numberOfWorker, out _numberOfWorker))
            { 
                _numberOfWorker = 1;
            }

            var delayStart = Helper.Helper.GetConfigValue(configuration, "DelayStart");

            if (!int.TryParse(delayStart, out _delayStart))
            {
                _delayStart = 5;
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                Task[] Consumer = new Task[_numberOfWorker];
                for (int i = 0; i < Consumer.Length; i++)
                {
                    Consumer[i] = Task.Factory.StartNew(async () =>
                    {
                        while (await _postChannel.Reader.WaitToReadAsync())
                        {
                            if (_postChannel.Reader.TryRead(out var channelData))
                            {
                                await Process(channelData);
                            }
                        }
                    });
                }
                Task.WaitAll(Consumer, stoppingToken);
            }
            await Task.CompletedTask;
        }

        private Task Process(Data post)
        {
            _logger.LogInformation($"Processing {post}");
            _postProcessor.ProcessPost(post);

            return Task.CompletedTask;
        }
    }
}
