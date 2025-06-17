using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SignalR.Services
{
    public class IaService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public IaService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClient = httpClientFactory.CreateClient();
            _config = config;
        }

        public async Task<string> ConsultarIa(string prompt)
        {
            var apiKey = _config["OpenRouter:ApiKey"];
            var baseUrl = _config["OpenRouter:BaseUrl"];
            var model = _config["OpenRouter:Model"];

            var payload = new
            {
                model = model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var jsonContent = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://github.com/alanpedrosa");
            _httpClient.DefaultRequestHeaders.Add("X-Title", "SignalRChat");

            var response = await _httpClient.PostAsync($"{baseUrl}/api/v1/chat/completions", content);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro na IA: {erro}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonResponse);

            return doc.RootElement
                      .GetProperty("choices")[0]
                      .GetProperty("message")
                      .GetProperty("content")
                      .GetString();
        }
    }
}
