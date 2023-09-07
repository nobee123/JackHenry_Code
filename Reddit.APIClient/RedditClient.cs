using Reddit.Models;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;

namespace Reddit.APIClient
{
    public interface IRedditClient
    {
        IEnumerable<Post> RetrieveSubredditPosts(string subreddit);
    }
    public class RedditClient : IRedditClient
    {      
        public IEnumerable<Post> RetrieveSubredditPosts(string subreddit)
        {
            try
            {
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://oauth.reddit.com/r/funny/new?limit=100&after=t3_16al61c")
                };

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsImtpZCI6IlNIQTI1NjpzS3dsMnlsV0VtMjVmcXhwTU40cWY4MXE2OWFFdWFyMnpLMUdhVGxjdWNZIiwidHlwIjoiSldUIn0.eyJzdWIiOiJ1c2VyIiwiZXhwIjoxNjk0MDY3MzUzLjA3NDE3OSwiaWF0IjoxNjkzOTgwOTUzLjA3NDE3OSwianRpIjoiSUdTTHYwYXhJdjY3MjB2aVFpWVpodkFndEFDd0lRIiwiY2lkIjoiaDFLaHZINFRyM0YzRmZEX3laUGZpZyIsImxpZCI6InQyXzNyd29nZTBoIiwiYWlkIjoidDJfM3J3b2dlMGgiLCJsY2EiOjE1NTc4OTg4OTQ0NDQsInNjcCI6ImVKeUtWdEpTaWdVRUFBRF9fd056QVNjIiwiZmxvIjo5fQ.VypuAz8Ryjw-zvdLFImBF_traObU9BrffHRgwILXJtonWaC-l-mms0aXOJn9J66CtOiHijMJUtphZQvulW_5RRIMOa7717JchQaJZqvquzY8MYA45yunzgHfRtUmRMTCtPRHCGg385No3AldA7Yu-TiO2YnB-xOjp7UYOQvx-258O2dg_zhXhl3m4vxrW10x6mQPgjPmWAct2uOo9ItqKr1UrSIj61iT2p8uZAcngwzGxXmKQeyHGPud90BZ0LFGKfH4nkQyJxjwSJH5YiC0hbjEK8Er3gpRm3Ve_NiHl19UNCgsPTlbsdJ4vwHb-JNdF9SY_5zt9xCI_l3XdI1XuA");
                requestMessage.Headers.UserAgent.Add(new ProductInfoHeaderValue("test", "1"));
                requestMessage.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "limit","100" }
        });
                var response = await httpClient.SendAsync(requestMessage);

                var testing = response.Content.ReadAsStringAsync();

            }
            catch (Exception ex)
            {

            }
        }
    }
}