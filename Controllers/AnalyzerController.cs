using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Mindly.Services;
using Mindly.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class AnalyzeController : ControllerBase
{
    private readonly MindlyAiService _service;
    public AnalyzeController(MindlyAiService service) => _service = service;

    [HttpPost]
    [AdminProtect]
    public async Task<IActionResult> Post([FromBody] AnalyzeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "O campo 'text' é obrigatório." });
        var result = await _service.AnalyzeAsync(request.Text);
        if (result == null)
            return StatusCode(502, new { error = "Erro ao consultar serviço externo." });
        return Ok(result);
    }
}

public class AnalyzeRequest
{
    [System.ComponentModel.DataAnnotations.Required]
    public string Text { get; set; }
}