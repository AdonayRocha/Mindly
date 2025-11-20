using Mindly.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mindly.Models;
using Mindly.Data;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class PsicologosController : ControllerBase
{
    private readonly MindlyContext _context;
    private readonly ILogger<PsicologosController> _logger;
    public PsicologosController(MindlyContext context, ILogger<PsicologosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost]
        [AdminProtect]
    public async Task<ActionResult<Psicologo>> Registrar([FromBody] Psicologo psicologo)
    {
        _context.Psicologos.Add(psicologo);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Psicologo registrado com id {Id}", psicologo.Id);
        return CreatedAtAction(nameof(GetById), new { id = psicologo.Id }, psicologo);
    }

    [HttpGet]
    [AdminProtect]
    public async Task<ActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Listando psicologos: page {Page}, pageSize {PageSize}", page, pageSize);
        var total = await _context.Psicologos.CountAsync();
        var psicologos = await _context.Psicologos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var result = new
        {
            total,
            page,
            pageSize,
            data = psicologos,
            links = new
            {
                self = Url.Action(nameof(Get), new { page, pageSize }),
                next = page * pageSize < total ? Url.Action(nameof(Get), new { page = page + 1, pageSize }) : null,
                prev = page > 1 ? Url.Action(nameof(Get), new { page = page - 1, pageSize }) : null
            }
        };
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AdminProtect]
    public async Task<ActionResult> GetById(int id)
    {
        var psicologo = await _context.Psicologos.FindAsync(id);
        if (psicologo == null)
        {
            _logger.LogWarning("Psicologo não encontrado: {Id}", id);
            return NotFound();
        }
        var result = new
        {
            data = psicologo,
            links = new
            {
                self = Url.Action(nameof(GetById), new { id }),
                update = Url.Action(nameof(Put), new { id }),
                delete = Url.Action(nameof(Delete), new { id })
            }
        };
        return Ok(result);
    }

    [HttpPut("{id}")]
    [AdminProtect]
    public async Task<IActionResult> Put(int id, Psicologo psicologo)
    {
        if (id != psicologo.Id)
        {
            _logger.LogWarning("Tentativa de atualizar psicologo com id inconsistente: {Id}", id);
            return BadRequest();
        }
        _context.Entry(psicologo).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Psicologo atualizado: {Id}", id);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Psicologos.Any(e => e.Id == id))
            {
                _logger.LogWarning("Psicologo não encontrado para update: {Id}", id);
                return NotFound();
            }
            throw;
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    [AdminProtect]
    public async Task<IActionResult> Delete(int id)
    {
        var psicologo = await _context.Psicologos.FindAsync(id);
        if (psicologo == null)
        {
            _logger.LogWarning("Psicologo não encontrado para delete: {Id}", id);
            return NotFound();
        }
        _context.Psicologos.Remove(psicologo);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Psicologo deletado: {Id}", id);
        return NoContent();
    }

}
