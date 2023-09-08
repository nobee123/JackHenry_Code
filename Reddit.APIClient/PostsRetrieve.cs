using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Reddit.APIClient
{
    public interface IPostsRetrieve
    {
        Task<string> RetrieveSubredditPostsAsync(string subreddit, string startingPoint);
    }
    public class PostsRetrieve : IPostsRetrieve
    {
        private int _numberOfRowToReturn;
        private string _baseUrl;
        private readonly HttpClient _httpClient;
        private readonly IRateLimitChecker _rateLimitChecker;
        private readonly ILogger<PostsRetrieve> _logger;
        private string _token;
        private readonly IAuthenticationService _authenticationService;
        private int _retrieveTokenRetries;
        private string _userAgent;

        public PostsRetrieve(IConfiguration configuration, HttpClient httpClient, IRateLimitChecker rateLimitChecker, ILogger<PostsRetrieve> logger, IAuthenticationService authenticationService) 
        {
            if (!string.IsNullOrWhiteSpace(configuration["NumberOfRowToReturn"]))
            {
                if (!int.TryParse(configuration["NumberOfRowToReturn"], out _numberOfRowToReturn))
                {
                    _numberOfRowToReturn = 100;
                }
            }
            _baseUrl = Helper.Helper.GetConfigValue(configuration, "SubredditAPIUrl");
            _userAgent = Helper.Helper.GetConfigValue(configuration, "UserAgent");

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
                    var jsonData = await _authenticationService.RetrieveAuthorizationsTokenAsync();
                    var token = JsonConvert.DeserializeObject<dynamic>(jsonData) ?? throw new Exception("Post Retrieve : Failed to set access token");
                    _token = token.access_token;
                }

                using (var request = new HttpRequestMessage(new HttpMethod("GET"), FormatUrl(subreddit, startingPoint)))
                {
                    request.Headers.TryAddWithoutValidation("User-Agent", _userAgent);
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

        private string FormatUrl(string subreddit, string startingPoint) => _baseUrl + "/r/" + subreddit + "/new?limit=" + _numberOfRowToReturn + "&after=" + startingPoint;       
    }
}