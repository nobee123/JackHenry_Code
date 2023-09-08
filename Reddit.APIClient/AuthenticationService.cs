using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Reddit.APIClient
{
    public interface IAuthenticationService
    {
        Task<string> RetrieveAuthorizationsTokenAsync();
    }
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _appId;
        private readonly string _appSecret;
        private readonly string _user;
        private readonly string _password;

        public AuthenticationService(ILogger<AuthenticationService> logger, IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;

            _appId = configuration["AppId"];
            _appSecret = configuration["AppSecret"];
            _user = configuration["User"];
            _password = configuration["Password"];

            _appId = Helper.Helper.GetConfigValue(configuration, "AppId");

            if(string.IsNullOrWhiteSpace(_appId))
                throw new ArgumentNullException("AppId");

            if (string.IsNullOrWhiteSpace(_appSecret))
                throw new ArgumentNullException("AppId");

            if (string.IsNullOrWhiteSpace(_user))
                throw new ArgumentNullException("AppId");

            if (string.IsNullOrWhiteSpace(_password))
                throw new ArgumentNullException("AppId");

        }

        public async Task<string> RetrieveAuthorizationsTokenAsync()
        {
            try
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://www.reddit.com/api/v1/access_token"))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", "Basic aDFLaHZINFRyM0YzRmZEX3laUGZpZzpSTG9rcUM1ZHFQclhWRzlIZUF6eWpVejRBVndrNkE=");

                    var contentList = new List<string>
                    {
                        $"grant_type={Uri.EscapeDataString("password")}",
                        $"username={Uri.EscapeDataString("Prudent_Comedian_624")}",
                        $"password={Uri.EscapeDataString("mdd$du+P7e/Uk&8")}"
                    };
                    request.Content = new StringContent(string.Join("&", contentList));
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                    var response = await _httpClient.SendAsync(request) ?? throw new Exception("Post Retrieve: Failed to get response.  Response is NULL");

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }                    

                    throw new Exception($"Authentication Service : Failed to token with following Status code: {response?.StatusCode}");
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
