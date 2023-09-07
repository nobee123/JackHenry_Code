using Reddit.Models;
using Reddit.Repository;

namespace Reddit.Logic
{
    public interface IPostWithMostUpVotes
    {       
        List<Data> Retrieve();
    }   
    public class PostWithMostUpVotes : IPostWithMostUpVotes
    {
        private readonly IPostRepository _repository;
        public PostWithMostUpVotes(IPostRepository postRepository) 
        {
            _repository = postRepository;
        }     

        public List<Data> Retrieve()
        {
            var posts = _repository.GetPosts();

            var result = posts.Select(x => x.Value);
            return result.AsParallel().OrderBy(x => x.ups).Take(10).ToList();
        }
    }
}