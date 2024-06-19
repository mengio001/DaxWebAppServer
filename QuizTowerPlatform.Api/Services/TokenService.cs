using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Configuration;

namespace QuizTowerPlatform.Api.Services
{
    public interface ITokenService
    {
        Task<string> GetTokenAsync();
    }

    public class TokenService : ITokenService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public TokenService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _config = configuration;
        }

        public async Task<string> GetTokenAsync()
        {
            // Voeg hier de logica toe om een token op te halen - machine-to-machine communication: "grant_type", "client_credentials".
            var response = await _httpClient.PostAsync("/connect/token", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _config.GetValue<string>("Application:IdPAudience")),
                new KeyValuePair<string, string>("client_secret", _config.GetValue<string>("Application:IdPAudienceSecret")),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", "towerofquizzesapi.read towerofquizzesapi.write")
            }));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);

            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

            return tokenResponse.AccessToken;
        }
    }
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
