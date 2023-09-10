using Moq;
using Reddit.Logic;
using Reddit.Models;
using Reddit.Repository;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reddit.Tests
{    
    public class UserWithMostPosts_Tests
    {
        private Mock<IPostRepository> _postRepository;

        [Fact]
        public void UserWithMostPosts_Should_Return_Top_10_User_With_Most_Posts()
        {

            _postRepository = new Mock<IPostRepository>();
            _postRepository.Setup(x => x.GetPosts()).Returns(SetupPosts());


            var userWithMostPosts = new UserWithMostPosts(_postRepository.Object);
            var post = userWithMostPosts.Retrieve();


            Assert.True(!post.Any(x => x.author == "l"));


        }

        private ConcurrentDictionary<string, Data> SetupPosts()
        {
            var testData = new ConcurrentDictionary<string, Data>();

            testData.TryAdd("1", new Data { id = "1", author = "a" });
            testData.TryAdd("7", new Data { id = "7", author = "a" });
            testData.TryAdd("2", new Data { id = "2", author = "b" });
            testData.TryAdd("3", new Data { id = "3", author = "b" });
            testData.TryAdd("4", new Data { id = "4", author = "c" });
            testData.TryAdd("5", new Data { id = "5", author = "c" });
            testData.TryAdd("6", new Data { id = "6", author = "d" });            
            testData.TryAdd("8", new Data { id = "8", author = "d" });
            testData.TryAdd("9", new Data { id = "9", author = "f" });
            testData.TryAdd("10", new Data { id = "10", author = "f" });
            testData.TryAdd("11", new Data { id = "11", author = "g" });
            testData.TryAdd("12", new Data { id = "12", author = "g" });
            testData.TryAdd("13", new Data { id = "13", author = "h" });
            testData.TryAdd("14", new Data { id = "14", author = "h" });
            testData.TryAdd("15", new Data { id = "15", author = "i" });
            testData.TryAdd("16", new Data { id = "16", author = "i" });
            testData.TryAdd("17", new Data { id = "17", author = "j" });
            testData.TryAdd("18", new Data { id = "18", author = "j" });
            testData.TryAdd("19", new Data { id = "19", author = "k" });
            testData.TryAdd("20", new Data { id = "20", author = "k" });
            testData.TryAdd("21", new Data { id = "21", author = "l" });            

            return testData;
        }
    }
}
