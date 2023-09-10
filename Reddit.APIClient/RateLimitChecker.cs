using Microsoft.Extensions.Logging;


namespace Reddit.APIClient
{
    public interface IRateLimitChecker
    {
        int RetrieverResetFromResponse(HttpResponseMessage response);
        bool NearLimitCheck(HttpResponseMessage response);
    }
    public class RateLimitChecker : IRateLimitChecker
    {
        private readonly ILogger<RateLimitChecker> _logger;
        public RateLimitChecker(ILogger<RateLimitChecker> logger) 
        {
            _logger = logger;   
        }
        public int RetrieverResetFromResponse(HttpResponseMessage response)
        {
            var resetLimitString = response.Headers.GetValues("X-Ratelimit-Reset").FirstOrDefault();
            var resetlimitInSecond = 40;

            if (string.IsNullOrWhiteSpace(resetLimitString))
            {
                _logger.LogWarning("Rate limit checker: can not retrieve rate limit reset time from response header");
                return resetlimitInSecond;
            }            

            if(int.TryParse(resetLimitString, out resetlimitInSecond)) 
            { 
                return resetlimitInSecond + 5;
            }

            return resetlimitInSecond;
        }       
        public bool NearLimitCheck(HttpResponseMessage response)
        {
            try
            {                 
                var remainingLimit = response.Headers.GetValues("X-Ratelimit-Remaining").FirstOrDefault();


                if (string.IsNullOrWhiteSpace(remainingLimit))
                    return false;

                if (Convert.ToInt32(remainingLimit) <= 50) 
                    return true;

                return false;
            }
            catch(Exception e) 
            {
                _logger.LogError(e, "Rate limit checker: Error in NearLimitCheck");
                throw;
            }
        }
    }
}
