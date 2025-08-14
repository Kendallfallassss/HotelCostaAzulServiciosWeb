using HotelCostaAzul.API.Models;
using HotelCostaAzul.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class HotelesController : Controller
{
    private readonly HotelifyContext _context;

    public HotelesController(HotelifyContext context)
    {
        _context = context;
    }

    // GET: /Hoteles
    public async Task<IActionResult> Index()
    {
        var hoteles = await _context.Hoteles.ToListAsync();
        return View(hoteles);
    }

    // GET: /Hoteles/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var hotel = await _context.Hoteles
        .Include(h => h.Habitaciones)
        .Include(h => h.Opiniones)
            .ThenInclude(o => o.Usuario)
        .FirstOrDefaultAsync(h => h.Id == id);

        if (hotel == null)
            return NotFound();

        return View(hotel);
    }

    //Crear hoteles
    public IActionResult Create()
    {
        return View();
    }

    // POST: Hoteles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Hotel hotel)
    {
        if (ModelState.IsValid)
        {
            _context.Hoteles.Add(hotel);
            _context.SaveChanges();
            return RedirectToAction(nameof(Gestion)); // o Index si no tenés Gestion
        }

        return View(hotel);
    }

    //Editar hoteles
    public IActionResult Edit(int id)
    {
        var hotel = _context.Hoteles.Find(id);
        if (hotel == null)
        {
            return NotFound();
        }
        return View(hotel);
    }

    // POST: Hoteles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Hotel hotel)
    {
        if (id != hotel.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(hotel);
                _context.SaveChanges();
                return RedirectToAction(nameof(Gestion));
            }
            catch
            {
                return Problem("Ocurrió un error al guardar los cambios.");
            }
        }

        return View(hotel);
    }

    //Eliminar hotel
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var hotel = await _context.Hoteles.FirstOrDefaultAsync(m => m.Id == id);
        if (hotel == null) return NotFound();

        return View(hotel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var hotel = await _context.Hoteles.FindAsync(id);
        _context.Hoteles.Remove(hotel);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    public IActionResult Gestion()
    {
        var hoteles = _context.Hoteles.ToList();
        return View(hoteles);
    }

    [HttpDelete]
    public IActionResult EliminarAjax(int id)
    {
        var hotel = _context.Hoteles.Find(id);
        if (hotel == null)
            return NotFound();

        _context.Hoteles.Remove(hotel);
        _context.SaveChanges();

        return Ok(new { success = true });
    }


}
