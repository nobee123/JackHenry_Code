using Reddit.Models;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

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
        public PostsRetrieve(IConfiguration configuration, HttpClient httpClient) 
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
        }
        
        public async Task<string> RetrieveSubredditPostsAsync(string subreddit, string startingPoint)
        {
            try
            {
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://oauth.reddit.com/r/funny/new?limit=100&after={startingPoint}")
                };

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsImtpZCI6IlNIQTI1NjpzS3dsMnlsV0VtMjVmcXhwTU40cWY4MXE2OWFFdWFyMnpLMUdhVGxjdWNZIiwidHlwIjoiSldUIn0.eyJzdWIiOiJ1c2VyIiwiZXhwIjoxNjk0MTU2MjI4LjIyNjM4MSwiaWF0IjoxNjk0MDY5ODI4LjIyNjM4MSwianRpIjoibm5DNXdkLWxjdDlPRi1UZnd2WktjRHhJal9CODN3IiwiY2lkIjoiaDFLaHZINFRyM0YzRmZEX3laUGZpZyIsImxpZCI6InQyXzNyd29nZTBoIiwiYWlkIjoidDJfM3J3b2dlMGgiLCJsY2EiOjE1NTc4OTg4OTQ0NDQsInNjcCI6ImVKeUtWdEpTaWdVRUFBRF9fd056QVNjIiwiZmxvIjo5fQ.dQxQ2oKy9YVQGSVUuF27nuba1-lRCox6hq3bm0zXrdfC9SUTUyrs3KHbivG-13Aznu_VLv8dcJdj7XK4V21QvPBa1iDY9kq7MRLU92-A-4tvDoUKKT1bFJXbPBzH8lidVi9tTvtUXly3J7j1Za12FuReHd1XPVZMQJA8880XhD1YQLMq8W59CjowZ7ff7ihhLGHidZNNCeA67jIxgo7F9OvxfEfc5kUuqsAQQWl1meuwFFdKE4hXXswTT1sjHCWfskeCgkLVeboVOf9hjv6rLZLhy4fGMBl8hEB_RopJPEW4umE-6kaZy2CxRjrp8S79d-X8XejDzgaCC6p55Ajn-Q");
                requestMessage.Headers.UserAgent.Add(new ProductInfoHeaderValue("test", "1"));                
                var response = await _httpClient.SendAsync(requestMessage);

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }
    }
}