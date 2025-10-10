namespace ApiFinanceira.Controllers;

using Contexts;
using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : Controller
{
    private readonly ApplicationDbContext _context;

    public UsuariosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/usuarios
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsuarios()
    {
        return await _context.Users.ToListAsync();
    }

    // GET: api/usuarios/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUsuario(int id)
    {
        var usuario = await _context.Users.FindAsync(id);

        if (usuario == null)
        {
            return NotFound();
        }

        return usuario;
    }

    // POST: api/usuarios
    [HttpPost]
    public async Task<ActionResult<User>> PostUsuario(User usuario)
    {
        _context.Users.Add(usuario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
    }
}