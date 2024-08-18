using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Configuration;
using System.Net;
using System.Security;

namespace QuizTowerPlatform.Api.Services
{
    public interface ITokenService
    {
        Task<string?> FetchTokenAsync(string clientId, string clientSecret, params string[] scopes);
    }

    /// <summary>
    /// Service om clientCredential tokens te verkrijgen. Tokens worden gecached indien mogelijk.
    /// Voorbeeld registratie van deze service:
    /// <code>
    /// services.AddHttpClient<ITokenService, TokenService>((sp, client) =>
    /// {
    ///     client.BaseAddress = new Uri(config.GetValue<string>("Application:Authority"));
    /// });
    /// </code>
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly HttpClient authority;
        private readonly IMemoryCache cache;

        public TokenService(HttpClient authority, IMemoryCache cache)
        {
            this.authority = authority;
            this.cache = cache;
        }

        public async Task<string?> FetchTokenAsync(string clientId, string clientSecret, params string[] scopes)
        {
            var key = new { clientId, clientSecret, scopes = string.Join(" ", scopes) };
            if (cache.TryGetValue(key, out string? token))
                return token;
            var tokenmodel = await GetAccessToken(clientId, clientSecret, scopes);
            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(tokenmodel.expires_in * 0.9));
            cache.Set(key, tokenmodel.access_token, options);
            token = tokenmodel.access_token;
            return token;
        }

        private async Task<Token> GetAccessToken(string clientId, string clientSecret, params string[] scopes)
        {
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentNullException(nameof(clientId));
            if (string.IsNullOrEmpty(clientSecret))
                throw new ArgumentNullException(nameof(clientSecret));
            if (clientSecret.StartsWith("#{") && clientSecret.EndsWith("}"))
                throw new ArgumentException($"Invalid clientSecret: '{clientSecret}'");
            using var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", clientId},
                { "client_secret", clientSecret},
                { "grant_type", "client_credentials"},
                { "scope", string.Join(" ", scopes)},
            });
            var response = await authority.PostAsync($"/connect/token", content);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var responseString = (await response.Content.ReadAsStringAsync());
                throw new SecurityException($"Error: StatusCode='{response.StatusCode}'. Request=[clientId={clientId}, Scopes={string.Join(" ", scopes)}] Response='{(responseString?.Length > 256 ? responseString.Substring(256) : responseString)}'");
            }
            return JsonConvert.DeserializeObject<Token>(await response.Content.ReadAsStringAsync())!;
        }

        public class Token
        {
            public string? access_token { get; set; }
            public int expires_in { get; set; }
            public string? token_type { get; set; }
            public string? scope { get; set; }
        }
    }
}
