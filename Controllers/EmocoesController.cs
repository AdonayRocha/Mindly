using Mindly.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class EmocoesController : ControllerBase
{
    private readonly ILogger<EmocoesController> _logger;
    public EmocoesController(ILogger<EmocoesController> logger) => _logger = logger;

    [HttpGet]
    [AdminProtect]
    public IActionResult Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        // Mock de emoções para exemplo
        var total = 5;
        var emocoes = new[]
        {
            new { Id = 1, Texto = "Feliz" },
            new { Id = 2, Texto = "Triste" },
            new { Id = 3, Texto = "Raiva" },
            new { Id = 4, Texto = "Ansiedade" },
            new { Id = 5, Texto = "Medo" }
        }
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();

        var result = new
        {
            total,
            page,
            pageSize,
            data = emocoes,
            links = new
            {
                self = Url.Action(nameof(Get), new { page, pageSize }),
                next = page * pageSize < total ? Url.Action(nameof(Get), new { page = page + 1, pageSize }) : null,
                prev = page > 1 ? Url.Action(nameof(Get), new { page = page - 1, pageSize }) : null
            }
        };
        return Ok(result);
    }

    [HttpPost]
    [AdminProtect]
    public async Task<IActionResult> RegistrarEmocao([FromBody] EmocaoRequest request)
    {
        bool alerta = false;
        string? riscoIa = null;
        if (!string.IsNullOrEmpty(request.Texto))
        {
            if (request.Texto.ToLower().Contains("matar"))
            {
                alerta = true;
                _logger.LogWarning("Alerta: palavra 'matar' detectada no texto do paciente.");
            }
            using var client = new HttpClient();
            var payload = new { text = request.Texto, meta = new { } };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://mindlyai.onrender.com/detect", content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                riscoIa = json;
            }
        }
        return Ok(new { sucesso = true, alerta, riscoIa });
    }
}

public class EmocaoRequest
{
    [System.ComponentModel.DataAnnotations.Required]
    public string? Texto { get; set; }
}