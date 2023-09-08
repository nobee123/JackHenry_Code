using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Reddit.APIClient
{
    public interface IPostsRetrieve
    {
        Task<string> RetrieveSubredditPostsAsync(string subreddit, string startingPoint);
    }
    public class PostsRetrieve : IPostsRetrieve
    {
        private string _numberOfRowToReturn = "100";
        private string _baseUrl;
        private readonly HttpClient _httpClient;
        private readonly IRateLimitChecker _rateLimitChecker;
        private readonly ILogger<PostsRetrieve> _logger;
        private string _token;
        private readonly IAuthenticationService _authenticationService;
        private int _retrieveTokenRetries;

        public PostsRetrieve(IConfiguration configuration, HttpClient httpClient, IRateLimitChecker rateLimitChecker, ILogger<PostsRetrieve> logger, IAuthenticationService authenticationService) 
        {
            if (!string.IsNullOrWhiteSpace(configuration["NumberOfRowToReturn"]))
            {
                _numberOfRowToReturn = configuration["NumberOfRowToReturn"];
            }

            if (!string.IsNullOrWhiteSpace(configuration["SubredditAPIUrl"]))
            {
                _baseUrl = configuration["SubredditAPIUrl"];
            }

            _httpClient = httpClient;
            _rateLimitChecker = rateLimitChecker;
            _logger = logger;
            _authenticationService = authenticationService;
        }
        
        public async Task<string> RetrieveSubredditPostsAsync(string subreddit, string startingPoint)
        {
            try
            {                
                if (string.IsNullOrWhiteSpace(_token))
                {
                    _token = await _authenticationService.RetrieveAuthorizationsTokenAsync();
                }

                using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://oauth.reddit.com/api/v1/me"))
                {
                    request.Headers.TryAddWithoutValidation("User-Agent", "ChangeMeClient/0.1 by YourUsername");
                    request.Headers.TryAddWithoutValidation("Authorization", $"bearer {_token}");

                    var response = await _httpClient.SendAsync(request) ?? throw new Exception("Post Retrieve: Failed to get response.  Response is NULL");
                    if (response.IsSuccessStatusCode)
                    {
                        if (_rateLimitChecker.NearLimitCheck(response))
                        {
                            await Task.Delay(TimeSpan.FromSeconds(_rateLimitChecker.RetrieverResetFromResponse(response)));
                        }
                        return await response.Content.ReadAsStringAsync();
                    }

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && _retrieveTokenRetries <= 3)
                    {
                        _retrieveTokenRetries++;
                        _logger.LogWarning($"Posts Retrieve: Failed to retrieve posts with status code {response.StatusCode} Retry : {_retrieveTokenRetries} ");
                        _token = string.Empty;
                        await RetrieveSubredditPostsAsync(subreddit, startingPoint);
                    }
                    else
                    {                        
                        throw new Exception($"Posts Retrieve : Failed to get token with following Status code: {response?.StatusCode}");
                    }

                    throw new Exception($"Posts Retrieve : Failed to retrieve posts with following Status code: {response?.StatusCode}");
                }              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Posts retrieve : Failed to retrieve data from Reddit API");
                throw;
            }        
        }       
    }
}