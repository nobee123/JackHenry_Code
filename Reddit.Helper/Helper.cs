using Microsoft.Extensions.Configuration;

namespace Reddit.Helper
{
    public static class Helper
    {   
        public static string GetConfigValue(IConfiguration configuration, string key)
        {
            var value = configuration[key];

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(key,$"Configuration does not contain {key} value");

            return value;
        }
    }
}
