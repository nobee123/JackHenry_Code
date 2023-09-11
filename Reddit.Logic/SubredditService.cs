using Microsoft.Extensions.Logging;
using Reddit.APIClient;
using Reddit.Models;
using System.Text.Json;

namespace Reddit.Logic
{
    public interface ISubredditService
    {
        Task<Child> ProcessAsync(string subredditName);
    }
    public class SubredditService : ISubredditService
    {
        private readonly ILogger<SubredditService> _logger;
        private string _startingPoint = "";
        private IPostsService _postsService;
        public SubredditService(ILogger<SubredditService> logger, IPostsService postsRetrieve)         
        {
            _logger = logger;
            _postsService = postsRetrieve;
        }

        public async Task<Child> ProcessAsync(string subredditName)
        {
            var result = await _postsService.RetrieveSubredditPostsAsync(subredditName, _startingPoint);

            var postData = JsonSerializer.Deserialize<Child>(result);

            if (postData != null)
            {
                _logger.LogInformation($"{subredditName} Subreddit Worker: Retrieve {postData?.data?.children?.Count}");
                if (postData == null)
                {
                    throw new Exception($"{subredditName} Subreddit Worker: Post data is null");
                }
                _startingPoint = postData.data.after;
                return postData;
            }
            else
            {
                _logger.LogInformation($"{subredditName} Subreddit Worker: did not retrieve any posts");
                return new Child();
            }
        }
    }
}
