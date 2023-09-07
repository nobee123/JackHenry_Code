using Reddit.Models;
using System.Collections.Concurrent;

namespace Reddit.Repository
{
    public interface IPostRepository
    {
        void Save(Data data);
        ConcurrentDictionary<string, Data> GetPosts();
    }
    public class PostRepository : IPostRepository
    {
        private ConcurrentDictionary<string, Data> _posts;

        public PostRepository() 
        {
            _posts = new ConcurrentDictionary<string, Data>();
        }

        public void Save(Data data)
        { 
            _posts.TryAdd(data.id, data);
        }

        public ConcurrentDictionary<string, Data> GetPosts() 
        {
            return _posts;        
        }
    }
}