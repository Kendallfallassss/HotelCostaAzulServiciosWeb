using HotelCostaAzul.API.Models;
using HotelCostaAzul.API.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class PagosController : Controller
{
    private readonly HotelifyContext _context;

    public PagosController(HotelifyContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Pagar(int id)
    {
        var factura = await _context.Facturaciones
            .Include(f => f.Reservacion)
            .Include(f => f.DetalleFacturas)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (factura == null) return NotFound();

        ViewBag.MetodosPago = new SelectList(await _context.MetodosPagos.ToListAsync(), "Id", "Tipo");

        return View(factura); // Vista: Views/Pagos/Pagar.cshtml
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmarPago(int facturaId, int metodoPagoId)
    {
        var factura = await _context.Facturaciones
            .Include(f => f.Pagos)
            .Include(f => f.DetalleFacturas)
            .FirstOrDefaultAsync(f => f.Id == facturaId);

        if (factura == null) return NotFound();

        if (factura.Estado == "Pagado")
            return RedirectToAction("Detalle", "Facturacion", new { id = factura.Id });

        var pago = new Pago
        {
            FacturacionId = factura.Id,
            ReservacionId = factura.ReservacionId,
            MetodoPagoId = metodoPagoId,
            MontoTotal = factura.MontoTotal, // o parcial si se soporta
            FechaPago = DateTime.UtcNow
        };

        _context.Pagos.Add(pago);

        // Cambiar estado si se pagó completo
        var totalPagado = factura.Pagos.Sum(p => p.MontoTotal) + pago.MontoTotal;
        if (totalPagado >= factura.MontoTotal)
        {
            factura.Estado = "Pagado";
        }

        await _context.SaveChangesAsync();

        TempData["PagoConfirmado"] = true;
        return RedirectToAction("Detalle", "Facturacion", new { id = factura.Id });
    }
}
