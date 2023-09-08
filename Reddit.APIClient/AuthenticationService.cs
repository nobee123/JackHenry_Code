using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reddit.APIClient
{
    public interface IAuthenticationService
    { 
    
    }
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public AuthenticationService(ILogger<AuthenticationService> logger, IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public string RetrieveAuthorizationsToken()
        {
            try
            {
                return null;
            }
            catch (Exception ex) 
            {
                
            
            }
            return null;
        }
    }
}
