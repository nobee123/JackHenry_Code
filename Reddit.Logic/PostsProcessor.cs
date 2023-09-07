using Reddit.Models;
using Reddit.Repository;

namespace Reddit.Logic
{
    public interface IPostsProcessor
    { 
        void ProcessPost(Data data);
    }
    public class PostsProcessor : IPostsProcessor
    {
        private readonly IPostRepository _postRepository;
        public PostsProcessor(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public void ProcessPost(Data data)
        {
            _postRepository.Save(data);
        }
    }
}
