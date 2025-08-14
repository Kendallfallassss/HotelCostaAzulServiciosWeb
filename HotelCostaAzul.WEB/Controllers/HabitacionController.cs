using HotelCostaAzul.API.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using HotelCostaAzul.API.Models;
using Microsoft.EntityFrameworkCore;

[Route("Habitaciones")]
public class HabitacionesController : Controller
{
    private readonly HotelifyContext _context;

    public HabitacionesController(HotelifyContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? hotelId)
    {
        var hoteles = await _context.Hoteles.ToListAsync();
        ViewBag.Hoteles = new SelectList(hoteles, "Id", "Nombre");

        var habitaciones = _context.Habitaciones
            .Include(h => h.Hotel)
            .AsQueryable();

        if (hotelId.HasValue)
        {
            habitaciones = habitaciones.Where(h => h.HotelId == hotelId);
            ViewBag.HotelSeleccionado = hotelId.Value;
        }

        return View(await habitaciones.ToListAsync());
    }



    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Hoteles = new SelectList(await _context.Hoteles.ToListAsync(), "Id", "Nombre");
        return View();
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Habitacion habitacion)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Hoteles = new SelectList(await _context.Hoteles.ToListAsync(), "Id", "Nombre");
            return View(habitacion);
        }

        _context.Habitaciones.Add(habitacion);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var habitacion = await _context.Habitaciones.FindAsync(id);
        if (habitacion == null) return NotFound();

        ViewBag.Hoteles = new SelectList(await _context.Hoteles.ToListAsync(), "Id", "Nombre", habitacion.HotelId);
        return View(habitacion);
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Habitacion habitacion)
    {
        if (id != habitacion.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            ViewBag.Hoteles = new SelectList(await _context.Hoteles.ToListAsync(), "Id", "Nombre", habitacion.HotelId);
            return View(habitacion);
        }

        _context.Update(habitacion);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var habitacion = await _context.Habitaciones
            .Include(h => h.Hotel)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (habitacion == null) return NotFound();

        return View(habitacion);
    }

    [HttpPost("Delete/{id}"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmDelete(int id)
    {
        var habitacion = await _context.Habitaciones
            .Include(h => h.Reservaciones)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (habitacion == null) return NotFound();

        if (habitacion.Reservaciones.Any())
        {
            TempData["Error"] = "❌ No se puede eliminar: tiene reservaciones asociadas.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        _context.Habitaciones.Remove(habitacion);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("EliminarAjax/{id}")]
    public async Task<IActionResult> EliminarAjax(int id)
    {
        try
        {
            var habitacion = await _context.Habitaciones
                .Include(h => h.Reservaciones)
                    .ThenInclude(r => r.Facturaciones)
                        .ThenInclude(f => f.Pagos)
                .Include(h => h.Reservaciones)
                    .ThenInclude(r => r.Facturaciones)
                        .ThenInclude(f => f.DetalleFacturas)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (habitacion == null)
                return NotFound("❌ La habitación no existe.");

            if (habitacion.Reservaciones.Any())
                return BadRequest("⚠️ No se puede eliminar: la habitación tiene reservaciones asociadas.");

            _context.Habitaciones.Remove(habitacion);
            await _context.SaveChangesAsync();

            return Ok("✅ Habitación eliminada correctamente.");
        }
        catch (DbUpdateException dbex)
        {
            Console.WriteLine("🚨 DbUpdateException:");
            Console.WriteLine(dbex.InnerException?.Message ?? dbex.Message);
            return BadRequest("❌ Esta habitación no puede eliminarse porque está relacionada con otros datos.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("🔥 Error inesperado:");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            return StatusCode(500, "🔥 Error inesperado al eliminar la habitación.");
        }
    }




    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var habitacion = await _context.Habitaciones
            .Include(h => h.Hotel)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (habitacion == null) return NotFound();

        return View(habitacion);
    }
}