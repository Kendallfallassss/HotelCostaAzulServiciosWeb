using HotelCostaAzul.API.Models;
using HotelCostaAzul.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;

public class FacturacionController : Controller
{
    private readonly HotelifyContext _context;

    public FacturacionController(HotelifyContext context)
    {
        _context = context;
    }

    // ✅ Mostrar detalle de una factura
    public async Task<IActionResult> Detalle(int id)
    {
        var factura = await _context.Facturaciones
        .Include(f => f.Reservacion)
            .ThenInclude(r => r.Habitacion)
            .ThenInclude(h => h.Hotel)
        .Include(f => f.DetalleFacturas)
        .Include(f => f.Pagos)
        .FirstOrDefaultAsync(f => f.Id == id);

        if (factura == null)
            return NotFound();

        return View(factura); // Vista: Views/Facturacion/Detalle.cshtml
    }

    // ✅ (Opcional) Lista de facturas del usuario logueado
    [Authorize]
    public async Task<IActionResult> MisFacturas()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        int userId = int.Parse(userIdClaim.Value);

        var facturas = await _context.Facturaciones
            .Include(f => f.Reservacion)
                .ThenInclude(r => r.Habitacion)
            .Where(f => f.Reservacion.UsuarioId == userId)
            .ToListAsync();

        return View(facturas); // Vista: Views/Facturacion/MisFacturas.cshtml
    }

    // ✅ (Desde TempData) crear factura luego de reservar
    [HttpGet("Reservaciones/Generar")]
    public async Task<IActionResult> Generar(string monto, int reservacionId)
    {
        if (string.IsNullOrEmpty(monto) || reservacionId == 0)
            return RedirectToAction("Index", "Home");

        decimal montoDecimal = decimal.Parse(monto, CultureInfo.InvariantCulture);

        var yaExiste = await _context.Facturaciones.AnyAsync(f => f.ReservacionId == reservacionId);
        if (yaExiste)
        {
            var existente = await _context.Facturaciones
                .FirstOrDefaultAsync(f => f.ReservacionId == reservacionId);

            return RedirectToAction("Detalle", new { id = existente.Id });
        }

        var reservacion = await _context.Reservaciones
            .Include(r => r.Habitacion)
            .FirstOrDefaultAsync(r => r.Id == reservacionId);

        if (reservacion == null)
            return NotFound();

        var factura = new Facturacion
        {
            ReservacionId = reservacionId,
            MontoTotal = montoDecimal,
            Estado = "Pendiente",
            NumeroFactura = "FAC-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
            Fecha = DateTime.UtcNow,
            DetalleFacturas = new List<DetalleFactura>
        {
            new DetalleFactura
            {
                Descripcion = $"Hospedaje del {reservacion.FechaInicio:dd/MM/yyyy} al {reservacion.FechaFin:dd/MM/yyyy} " +
                              $"en habitación #{reservacion.Habitacion?.Numero} para {reservacion.CantidadPersonas} persona(s).",
                Monto = montoDecimal
            }
        }
        };

        _context.Facturaciones.Add(factura);
        await _context.SaveChangesAsync();

        return RedirectToAction("Detalle", new { id = factura.Id });
    }

}
