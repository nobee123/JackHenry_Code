using Moq;
using Reddit.Logic;
using Reddit.Models;
using Reddit.Repository;
using System.Collections.Concurrent;

namespace reddit.Tests
{
    public class PostWithMostUpVotes_Tests
    {
        private Mock<IPostRepository> _postRepository;

        [Fact]
        public void PostWithMostUpVotes_Should_Return_Top_10_Post_With_Most_Up_Votes()
        {
            
            _postRepository = new Mock<IPostRepository>();
            _postRepository.Setup(x => x.GetPosts()).Returns(SetupPosts());


            var postWithMostUpVotes = new PostWithMostUpVotes(_postRepository.Object);
            var post = postWithMostUpVotes.Retrieve();


            Assert.True(!post.Any(x => x.id == "11"));


        }

        private ConcurrentDictionary<string, Data> SetupPosts()
        {
            var testData = new ConcurrentDictionary<string, Data>();

            testData.TryAdd("1", new Data { id = "1", ups= 111 });
            testData.TryAdd("2", new Data { id = "2", ups = 110 });
            testData.TryAdd("3", new Data { id = "3", ups = 110 });
            testData.TryAdd("4", new Data { id = "4", ups = 110 });
            testData.TryAdd("5", new Data { id = "5", ups = 110 });
            testData.TryAdd("6", new Data { id = "6", ups = 110 });
            testData.TryAdd("7", new Data { id = "7", ups = 110 });
            testData.TryAdd("8", new Data { id = "8", ups = 110 });
            testData.TryAdd("9", new Data { id = "9", ups = 110 });
            testData.TryAdd("10", new Data { id = "10", ups = 110 });
            testData.TryAdd("11", new Data { id = "11", ups = 99 });

            return testData;
        }
    }
}