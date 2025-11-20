using System.Net.Http.Headers;
using System.Text.Json;

namespace Mindly.Services
{
    public class MindlyAiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MindlyAiService> _logger;
        public MindlyAiService(HttpClient httpClient, ILogger<MindlyAiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.BaseAddress = new Uri("https://mindlyai.onrender.com/");
            _httpClient.DefaultRequestHeaders.Add("api-key", "admin");
        }

        public async Task<AnalyzeResponse> AnalyzeAsync(string text)
        {
            _logger.LogInformation("Enviando texto para análise na IA");
            var content = new StringContent(JsonSerializer.Serialize(new { text }), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("analyze", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Resposta recebida da IA");
            return JsonSerializer.Deserialize<AnalyzeResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public class AnalyzeResponse
        {
            public string Insights { get; set; }
        }
    }
}