using Microsoft.Extensions.Hosting;
using Reddit.Models;
using System.Threading.Channels;

namespace Reddit.HostedService
{
    public class PostProcessor : BackgroundService
    {
        private readonly Channel<Data> _postChannel;

        public PostProcessor(Channel<Data> postChannel)
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

        private Task DoSomeWork(Data post)
        {
            // process the post 
            return Task.CompletedTask;
        }
    }
}
