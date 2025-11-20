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
public class PacientesController : ControllerBase
{
    private readonly MindlyContext _context;
    private readonly ILogger<PacientesController> _logger;
    public PacientesController(MindlyContext context, ILogger<PacientesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost]
        [AdminProtect]
    public async Task<ActionResult<Paciente>> Registrar([FromBody] Paciente paciente)
    {
        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Paciente registrado com id {Id}", paciente.Id);
        return CreatedAtAction(nameof(GetById), new { id = paciente.Id }, paciente);
    }

    [HttpGet]
    [AdminProtect]
    public async Task<ActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Listando pacientes: page {Page}, pageSize {PageSize}", page, pageSize);
        var total = await _context.Pacientes.CountAsync();
        var pacientes = await _context.Pacientes
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var result = new
        {
            total,
            page,
            pageSize,
            data = pacientes,
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
        var paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null)
        {
            _logger.LogWarning("Paciente não encontrado: {Id}", id);
            return NotFound();
        }
        var result = new
        {
            data = paciente,
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
    public async Task<IActionResult> Put(int id, Paciente paciente)
    {
        if (id != paciente.Id)
        {
            _logger.LogWarning("Tentativa de atualizar paciente com id inconsistente: {Id}", id);
            return BadRequest();
        }
        _context.Entry(paciente).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Paciente atualizado: {Id}", id);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Pacientes.Any(e => e.Id == id))
            {
                _logger.LogWarning("Paciente não encontrado para update: {Id}", id);
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
        var paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null)
        {
            _logger.LogWarning("Paciente não encontrado para delete: {Id}", id);
            return NotFound();
        }
        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Paciente deletado: {Id}", id);
        return NoContent();
    }

}
