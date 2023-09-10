using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;

namespace Reddit.APIClient
{
    public interface IAuthenticationService
    {
        Task<string> RetrieveAuthorizationsTokenAsync();
    }
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _appId;
        private readonly string _appSecret;
        private readonly string _user;
        private readonly string _password;
        private readonly string _url;
        private readonly string _userAgent;

        public AuthenticationService(ILogger<AuthenticationService> logger, IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger;        
            _httpClient = httpClient;

            _appId = Helper.Helper.GetConfigValue(configuration, "AppId"); 
            _appSecret = Helper.Helper.GetConfigValue(configuration, "AppSecret");
            _user = Helper.Helper.GetConfigValue(configuration, "User");
            _password = Helper.Helper.GetConfigValue(configuration, "Password");
            _url = Helper.Helper.GetConfigValue(configuration, "RedditAccessTokenAPIUrl");
            _userAgent = Helper.Helper.GetConfigValue(configuration, "UserAgent");
        }

        public async Task<string> RetrieveAuthorizationsTokenAsync()
        {
            try
            {

                using (var request = new HttpRequestMessage(new HttpMethod("POST"), _url))
                {
                    var bearToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(_appId + ":" + _appSecret));
                    request.Headers.TryAddWithoutValidation("Authorization", $"Basic {bearToken}");
                    request.Headers.Add("User-Agent", _userAgent);

                    var contentList = new List<string>
                    {
                        $"grant_type={Uri.EscapeDataString("password")}",
                        $"username={Uri.EscapeDataString(_user)}",
                        $"password={Uri.EscapeDataString(_password)}"
                    };
                    request.Content = new StringContent(string.Join("&", contentList));
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                    var response = await _httpClient.SendAsync(request) ?? throw new Exception("Post Retrieve: Failed to get response.  Response is NULL");

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }

                    throw new Exception($"Authentication Service : Failed to retrieve token with following Status code: {response?.StatusCode}");
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Authentication Service : Failed to retrieve token for Reddit");
                throw;
            }           
        }
    }
}
