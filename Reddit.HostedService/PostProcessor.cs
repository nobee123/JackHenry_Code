using Microsoft.Extensions.Hosting;
using Reddit.Models;
using System.Threading.Channels;

namespace Reddit.HostedService
{
    public class PostProcessor : BackgroundService
    {
        private readonly Channel<Post> _postChannel;

        public PostProcessor(Channel<Post> postChannel)
        {
            _postChannel = postChannel;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Task[] Consumer = new Task[12];
                for (int i = 0; i < Consumer.Length; i++)
                {
                    Consumer[i] = Task.Factory.StartNew(async () =>
                    {
                        while (await _postChannel.Reader.WaitToReadAsync())
                        {
                            if (_postChannel.Reader.TryRead(out var channelData))
                            {
                                await DoSomeWork(channelData);
                            }
                        }
                    });
                }
                Task.WaitAll(Consumer, stoppingToken);
            }
        }

        private Task DoSomeWork(Post post)
        {
            return Task.CompletedTask;
        }
    }
}
