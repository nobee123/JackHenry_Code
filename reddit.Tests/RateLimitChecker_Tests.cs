using Microsoft.Extensions.Logging;
using Moq;
using Reddit.APIClient;

namespace reddit.Tests
{
    public class RateLimitChecker_Tests
    {

        [Fact]
        public void Checker_Verify_Response_Near_Limit()
        {
            var logger = new Mock<ILogger<RateLimitChecker>>();
            var rateLimitChecker = new RateLimitChecker(logger.Object);
            var message = new HttpResponseMessage();         
            message.Headers.Add("X-Ratelimit-Remaining", "50");
            var result = rateLimitChecker.NearLimitCheck(message);
            Assert.True(result);

        }

        [Fact]
        public void Checker_Verify_Response_Not_Near_Limit()
        {
            var logger = new Mock<ILogger<RateLimitChecker>>();
            var rateLimitChecker = new RateLimitChecker(logger.Object);
            var message = new HttpResponseMessage();
            message.Headers.Add("X-Ratelimit-Remaining", "55");
            var result = rateLimitChecker.NearLimitCheck(message);
            Assert.True(!result);

        }
    }
}
