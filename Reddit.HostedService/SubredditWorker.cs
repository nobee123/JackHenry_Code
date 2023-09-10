﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reddit.APIClient;
using Reddit.Logic;
using Reddit.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;

namespace Reddit.HostedService
{
    public class SubredditWorker : BackgroundService
    {
        private readonly ILogger<SubredditWorker> _logger;        
        private readonly Channel<Data> _postChannel;
        private readonly IPostsService _postsRetrieve;
        private readonly string _subredditName = "funny";
        private readonly int _delayStart = 5;
        private readonly ISubredditService _subredditService;

        public SubredditWorker(ILogger<SubredditWorker> logger, Channel<Data> postChannel, ISubredditService subredditService,  IConfiguration configuration)
        {
            _logger = logger;            
            _postChannel = postChannel;
            _subredditService = subredditService;
            var delayStart = configuration["DelayStart"];

            if (!int.TryParse(delayStart, out _delayStart))
            {
                logger.LogWarning("Subreddit Worker : Can not get delay start value default back to 5 ");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Subreddit worker is running");
                       
            await Task.Delay(TimeSpan.FromSeconds(_delayStart), stoppingToken);

            while (!stoppingToken.IsCancellationRequested && await _postChannel.Writer.WaitToWriteAsync(stoppingToken))
            {
                try
                {
                    var data = await _subredditService.ProcessAsync(_subredditName);

                    foreach(var item in data.data.children) 
                    {
                        await _postChannel.Writer.WriteAsync(item.data, stoppingToken);
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