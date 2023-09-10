using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Reddit.APIClient;
using Reddit.HostedService;
using Reddit.Logic;
using Reddit.Models;
using Reddit.Repository;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace reddit.Tests
{
    public class FunnySubredditWorker_Tests
    {
        private Mock<IPostRepository> _postRepository;

        [Fact]
        public void FunnySubredditWorker_Channel_Should_Contain_Same_Amount_Of_Message()
        {
            var logger = new Mock<ILogger<SubredditHostedService>>();
            var channel = new Mock<Channel<Data>>();
            channel.Setup(x => x.Writer.WriteAsync(It.IsAny<Data>(), CancellationToken.None));

            var postService = new Mock<IPostsService>();
            var configuration = new Mock<IConfiguration>();

            var mockIConfigurationSection = new Mock<IConfigurationSection>();                       

            configuration.Setup(x => x["DelayStart"]).Returns("0");



            var worker = new SubredditWorker(logger.Object, channel.Object, postService.Object, configuration.Object);

            worker.StartAsync(CancellationToken.None).Wait();

            

            
        }

        private ConcurrentDictionary<string, Data> SetupPosts()
        {
            var testData = new ConcurrentDictionary<string, Data>();

            testData.TryAdd("1", new Data { id = "1", ups = 111 });
            testData.TryAdd("2", new Data { id = "2", ups = 110 });
            testData.TryAdd("3", new Data { id = "3", ups = 110 });
            testData.TryAdd("4", new Data { id = "4", ups = 110 });
            testData.TryAdd("5", new Data { id = "5", ups = 110 });
            testData.TryAdd("6", new Data { id = "6", ups = 110 });
            testData.TryAdd("7", new Data { id = "7", ups = 110 });
            testData.TryAdd("8", new Data { id = "8", ups = 110 });
            testData.TryAdd("9", new Data { id = "9", ups = 110 });
            testData.TryAdd("10", new Data { id = "10", ups = 110 });
            testData.TryAdd("11", new Data { id = "11", ups = 99 });

            return testData;
        }
    }
}
