using Mindly.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Mindly.Models;
using Mindly.Data;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class RotinasController : ControllerBase
{
    private readonly MindlyContext _context;
    public RotinasController(MindlyContext context) => _context = context;

    [HttpGet]
        
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var total = await _context.Rotinas.CountAsync();
        var rotinas = await _context.Rotinas.Include(r => r.Paciente)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var result = new
        {
            total,
            page,
            pageSize,
            data = rotinas,
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
    public async Task<IActionResult> GetById(int id)
    {
        var rotina = await _context.Rotinas.Include(r => r.Paciente).FirstOrDefaultAsync(r => r.Id == id);
        if (rotina == null) return NotFound();
        var result = new
        {
            data = rotina,
            links = new
            {
                self = Url.Action(nameof(GetById), new { id }),
                update = Url.Action(nameof(Put), new { id }),
                delete = Url.Action(nameof(Delete), new { id })
            }
        };
        return Ok(result);
    }

    [HttpPost]
    [AdminProtect]
    public async Task<IActionResult> Post([FromBody] Rotina rotina)
    {
        // Verifica se o paciente existe antes de criar a rotina
        var pacienteExiste = await _context.Pacientes.AnyAsync(p => p.Id == rotina.PacienteId);
        if (!pacienteExiste)
        {
            return BadRequest("Paciente não encontrado. Não é possível criar rotina para paciente inexistente.");
        }
        rotina.Data = rotina.Data == default ? DateTime.UtcNow : rotina.Data;
        _context.Rotinas.Add(rotina);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = rotina.Id }, rotina);
    }

    [HttpPut("{id}")]
    [AdminProtect]
    public async Task<IActionResult> Put(int id, [FromBody] Rotina rotina)
    {
        if (id != rotina.Id) return BadRequest();
        _context.Entry(rotina).State = EntityState.Modified;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Rotinas.Any(r => r.Id == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    [AdminProtect]
    public async Task<IActionResult> Delete(int id)
    {
        var rotina = await _context.Rotinas.FindAsync(id);
        if (rotina == null) return NotFound();
        _context.Rotinas.Remove(rotina);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}