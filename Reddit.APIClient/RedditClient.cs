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
            throw new NotImplementedException();
        }
    }
}