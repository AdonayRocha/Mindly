using Mindly.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Mindly.Services;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class EmocoesController : ControllerBase
{
    private readonly ILogger<EmocoesController> _logger;
    private readonly MindlyAiService _ai;
    public EmocoesController(ILogger<EmocoesController> logger, MindlyAiService ai)
    {
        _logger = logger;
        _ai = ai;
    }

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

    /// <summary>
    /// Detecta risco e tópicos emocionais a partir de um texto.
    /// </summary>
    /// <remarks>
    /// Este endpoint envia o texto para o serviço externo (/detect) e retorna um indicador de alerta,
    /// palavras de alerta detectadas, tópicos identificados e uma pontuação de risco. Também marcamos
    /// alerta localmente se palavras explícitas de violência forem encontradas.
    /// </remarks>
    /// <param name="request">Texto a ser avaliado</param>
    /// <response code="200">Retorna status de sucesso, alerta consolidado e o objeto de detecção</response>
    [HttpPost]
    [AdminProtect]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> RegistrarEmocao([FromBody] EmocaoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Texto))
        {
            return BadRequest(new { erro = "Texto é obrigatório." });
        }

        var texto = request.Texto.Trim();
        var alertaLocal = texto.ToLower().Contains("matar") || texto.ToLower().Contains("suicida");
        if (alertaLocal)
        {
            _logger.LogWarning("Alerta local: termos de violência/suicídio detectados no texto.");
        }

        var detect = await _ai.DetectAsync(texto);
        var alerta = alertaLocal || (detect?.Alert ?? false);

        return Ok(new
        {
            sucesso = true,
            alerta,
            riscoIa = new
            {
                alert = detect?.Alert ?? false,
                alert_words = detect?.Alert_Words ?? Array.Empty<string>(),
                topics = detect?.Topics ?? Array.Empty<string>(),
                risk_score = detect?.Risk_Score ?? 0
            }
        });
    }
}

public class EmocaoRequest
{
    [System.ComponentModel.DataAnnotations.Required]
    public string? Texto { get; set; }
}