using HotelCostaAzul.API.Models.Entities;
using HotelCostaAzul.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
public class OpinionesController : Controller
{
    private readonly HotelifyContext _context;

    public OpinionesController(HotelifyContext context)
    {
        _context = context;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Opinion opinion)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        opinion.UsuarioId = int.Parse(userId);
        opinion.Fecha = DateTime.UtcNow;

        _context.Opiniones.Add(opinion);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Hoteles", new { id = opinion.HotelId });
    }
}
