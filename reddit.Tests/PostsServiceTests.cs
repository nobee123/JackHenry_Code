using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Reddit.APIClient;
using System.Net.Http.Headers;

namespace reddit.Tests
{
    public class PostsServiceTests
    {
        private MockRepository mockRepository;

        private Mock<IConfiguration> mockConfiguration;
        private HttpClient httpClient;
        private Mock<IRateLimitChecker> mockRateLimitChecker;
        private Mock<ILogger<PostsService>> mockLogger;
        private Mock<IAuthenticationService> mockAuthenticationService;

        public PostsServiceTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Loose);

            this.mockConfiguration = this.mockRepository.Create<IConfiguration>();

            mockConfiguration.Setup(x => x["SubredditAPIUrl"]).Returns("http://Testing.com");
            mockConfiguration.Setup(x => x["UserAgent"]).Returns("test");
           
            var httpMessageHandler = new Mock<HttpMessageHandler>();

            httpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                    )
                    .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                    {
                        HttpResponseMessage response = new HttpResponseMessage();
                        response.StatusCode = System.Net.HttpStatusCode.OK;  
                        response.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject("test")); 
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        response.Content.Headers.Add("X-Ratelimit-Reset", "40");
                        response.Content.Headers.Add("X-Ratelimit-Remaining", "100");
                        return response;
                    });
            httpClient = new HttpClient(httpMessageHandler.Object);
            this.mockRateLimitChecker = this.mockRepository.Create<IRateLimitChecker>();
            this.mockLogger = this.mockRepository.Create<ILogger<PostsService>>();
            this.mockAuthenticationService = this.mockRepository.Create<IAuthenticationService>();
            mockAuthenticationService.Setup(x => x.RetrieveAuthorizationsTokenAsync()).Returns(Task.FromResult("{\"access_token\": \"token\", \"token_type\": \"bearer\", \"expires_in\": 86400, \"scope\": \"*\"}\n"));
                
        }

        private PostsService CreateService()
        {    
            return new PostsService(
                mockConfiguration.Object,
                httpClient,
                mockRateLimitChecker.Object,
                mockLogger.Object,
                mockAuthenticationService.Object);                 
        }

        [Fact]
        public async Task RetrieveSubredditPostsAsync_Authorization_Token_Is_Called()
        {
            // Arrange
            var service = this.CreateService();
            string subreddit = null;
            string startingPoint = null;

            // Act
            var result = await service.RetrieveSubredditPostsAsync(
                subreddit,
                startingPoint);

            // Assert

            mockAuthenticationService.Verify(x => x.RetrieveAuthorizationsTokenAsync(), Times.Once());
        }

        [Fact]
        public async Task RetrieveSubredditPostsAsync_Rate_Limit_Check_Should_Pass()
        { 
            var rateLimitChecker = new Mock<IRateLimitChecker>();
            rateLimitChecker.Setup(x => x.NearLimitCheck(It.IsAny<HttpResponseMessage>())).Returns(false);

            var postsService = new PostsService(mockConfiguration.Object, httpClient, rateLimitChecker.Object, mockLogger.Object, mockAuthenticationService.Object);

            string subreddit = null;
            string startingPoint = null;

            // Act
            var result = await postsService.RetrieveSubredditPostsAsync(
                subreddit,
                startingPoint);

            // Assert

            rateLimitChecker.Verify(x => x.RetrieverResetFromResponse(It.IsAny<HttpResponseMessage>()), Times.Never);
        }
    }
}
