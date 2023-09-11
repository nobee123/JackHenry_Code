using Reddit.Models;
using Reddit.Repository;

namespace Reddit.Logic
{
    public interface IUserWithMostPosts
    {        
        List<Data> Retrieve();

    }
    public class UserWithMostPosts : IUserWithMostPosts
    {
        private readonly IPostRepository _repository;
        public UserWithMostPosts(IPostRepository postRepository) 
        {
            _repository = postRepository;
        }
        public List<Data> Retrieve()
        {
            var postDictionary =_repository.GetPosts();

            var result = postDictionary.Select(x => x.Value);            

            return result.AsParallel()
                               .GroupBy(x => x.author)
                               .OrderByDescending(c => c.Count())
                               .Take(10)
                               .Select(group => new Data { author = group.Key, Count = group.Count()  })
                               .ToList();           
        }
    }
}
