using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SignalR.Controllers
{
    [ApiController]
    [Route("api/ia")]
    public class IaController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public IaController(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _http = httpClientFactory.CreateClient();
        }

        [HttpPost("comando")]
        public async Task<IActionResult> Comando([FromBody] JsonElement body)
        {
            var comando = body.GetProperty("comando").GetString();
            var resposta = await ConsultarIaAsync(comando);
            return Ok(new { resposta });
        }

        [HttpPost("resumo")]
        public async Task<IActionResult> Resumo([FromBody] JsonElement body)
        {
            var conversa = body.GetProperty("conversa").GetString();
            var prompt = $"Faça um resumo desta conversa de chat:\n{conversa}";
            var resumo = await ConsultarIaAsync(prompt);
            return Ok(new { resumo });
        }

        private async Task<string> ConsultarIaAsync(string prompt)
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

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var response = await _http.PostAsync($"{baseUrl}/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
        }
    }
}
