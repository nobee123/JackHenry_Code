using Reddit.Models;
using System.Collections.Generic;

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
            var payload = GeneratePayload();            

        }

        private RequestPayload GeneratePayload()
        {
            var requestPayload = new RequestPayload
            {
                after = "",
                before = "",
                limit = 100,
            };
            return requestPayload;
        }
    }
}