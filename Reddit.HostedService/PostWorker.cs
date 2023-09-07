using Microsoft.Extensions.Hosting;
using Reddit.Logic;
using Reddit.Models;
using System.Threading.Channels;

namespace Reddit.HostedService
{
    public class PostWorker : BackgroundService
    {
        private readonly Channel<Data> _postChannel;
        private readonly IPostsProcessor _postProcessor;

        public PostWorker(Channel<Data> postChannel, IPostsProcessor postsProcessor)
        {
            _postChannel = postChannel;
            _postProcessor = postsProcessor;
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
            _postProcessor.ProcessPost(post);
            return Task.CompletedTask;
        }
    }
}
