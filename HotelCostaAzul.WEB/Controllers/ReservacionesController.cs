using HotelCostaAzul.API.Models;
using HotelCostaAzul.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;

namespace Hotelify.Web.Controllers
{
    [Route("Reservaciones")]
    public class ReservacionesController : Controller
    {
        private readonly HotelifyContext _context;

        public ReservacionesController(HotelifyContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var lista = await _context.Reservaciones
                .Include(r => r.Habitacion)
                .Include(r => r.Usuario)
                .ToListAsync();

            return View(lista);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create(int? hotelId)
        {
            var habitaciones = await _context.Habitaciones
                .Where(h => h.Disponibilidad && (!hotelId.HasValue || h.HotelId == hotelId))
                .ToListAsync();

            ViewBag.Habitaciones = habitaciones;
            ViewBag.HotelId = hotelId;

            return View(new Reservacion
            {
                FechaInicio = DateOnly.FromDateTime(DateTime.Today),
                FechaFin = DateOnly.FromDateTime(DateTime.Today.AddDays(1))
            });
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservacion reservacion, string MontoCalculado, int? HotelId)
        {
            async Task<IActionResult> Error(string mensaje, int? hotelId = null)
            {
                ModelState.AddModelError("", mensaje);
                await CargarHabitaciones(hotelId ?? HotelId);
                ViewBag.HotelId = hotelId ?? HotelId;
                return View("Create", reservacion);
            }

            if (string.IsNullOrWhiteSpace(MontoCalculado))
                return await Error("Debe calcular el monto antes de enviar.");

            if (!decimal.TryParse(MontoCalculado, NumberStyles.Any, CultureInfo.InvariantCulture, out var monto))
                return await Error("El monto no es válido.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            reservacion.UsuarioId = int.Parse(userId);

            var habitacion = await _context.Habitaciones.FindAsync(reservacion.HabitacionId);
            if (habitacion == null)
                return await Error("La habitación no existe.");

            if (reservacion.CantidadPersonas > habitacion.Capacidad)
                return await Error($"La habitación permite máximo {habitacion.Capacidad} personas.", habitacion.HotelId);

            bool solapada = await _context.Reservaciones.AnyAsync(r =>
                r.HabitacionId == reservacion.HabitacionId &&
                reservacion.FechaInicio < r.FechaFin &&
                reservacion.FechaFin > r.FechaInicio
            );

            if (solapada)
                return await Error("Ya existe una reservación en esas fechas.", habitacion.HotelId);

            if (reservacion.FechaInicio >= reservacion.FechaFin)
                return await Error("La fecha de inicio debe ser anterior a la fecha de fin.", habitacion?.HotelId ?? HotelId);

            reservacion.FechaReserva = DateTime.UtcNow;
            reservacion.Monto = monto;

            _context.Reservaciones.Add(reservacion);
            await _context.SaveChangesAsync();

            TempData["MontoTotal"] = monto.ToString(CultureInfo.InvariantCulture);
            TempData["ReservacionId"] = reservacion.Id;

            return RedirectToAction("Generar", "Facturacion", new
            {
                monto = monto.ToString(CultureInfo.InvariantCulture),
                reservacionId = reservacion.Id
            });

        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var reservacion = await _context.Reservaciones
                .Include(r => r.Habitacion)
                .Include(r => r.Facturaciones)
                .FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == userId);

            if (reservacion == null)
                return Unauthorized();

            if (reservacion.Facturaciones.Any(f => f.Estado == "Pagado"))
            {
                TempData["Error"] = "❌ No se puede editar una reservación ya pagada.";
                return RedirectToAction("MisReservas");
            }

            // 📌 AQUÍ preparamos ViewBag.Habitaciones correctamente:
            var habitaciones = await _context.Habitaciones
                .Where(h => h.Disponibilidad)
                .Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = $"#{h.Numero} - ₡{h.PrecioPorPersona:N0}" // Texto bonito para el select
                }).ToListAsync();

            ViewBag.Habitaciones = habitaciones;

            // 🚀 Finalmente devolvemos la vista
            return View(reservacion);
        }




        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reservacion reservacion, string MontoCalculado)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var existente = await _context.Reservaciones
                .Include(r => r.Habitacion)
                .Include(r => r.Facturaciones) // 🆕 incluir la factura relacionada
                .FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == userId);

            if (existente == null) return Unauthorized();

            // Revalidar monto
            if (!decimal.TryParse(MontoCalculado, NumberStyles.Any, CultureInfo.InvariantCulture, out var monto))
            {
                ModelState.AddModelError("", "❌ El monto no es válido.");
                await CargarHabitaciones();
                return View(reservacion);
            }

            // Validar fechas solapadas
            var solapada = await _context.Reservaciones.AnyAsync(r =>
                r.Id != id &&
                r.HabitacionId == reservacion.HabitacionId &&
                reservacion.FechaInicio < r.FechaFin &&
                reservacion.FechaFin > r.FechaInicio
            );

            if (solapada)
            {
                ModelState.AddModelError("", "❌ Hay otra reservación en esas fechas.");
                await CargarHabitaciones();
                return View(reservacion);
            }

            // ✅ Actualizar reservación
            existente.FechaInicio = reservacion.FechaInicio;
            existente.FechaFin = reservacion.FechaFin;
            existente.CantidadPersonas = reservacion.CantidadPersonas;
            existente.Monto = monto;

            // Actualizar reservación
            existente.FechaInicio = reservacion.FechaInicio;
            existente.FechaFin = reservacion.FechaFin;
            existente.CantidadPersonas = reservacion.CantidadPersonas;
            existente.Monto = monto;

            // 🔥 Actualizar Factura y DetalleFactura si NO está pagada
            var factura = await _context.Facturaciones
                .Include(f => f.DetalleFacturas)
                .FirstOrDefaultAsync(f => f.ReservacionId == id);

            if (factura != null && factura.Estado != "Pagado")
            {
                // Actualiza el monto total de la factura
                factura.MontoTotal = monto;

                // Actualiza el primer detalle (por simplicidad)
                var detalle = factura.DetalleFacturas.FirstOrDefault();
                if (detalle != null)
                {
                    detalle.Monto = monto;
                    detalle.Descripcion = $"Hospedaje del {existente.FechaInicio:dd/MM/yyyy} al {existente.FechaFin:dd/MM/yyyy} en habitación #{existente.Habitacion?.Numero} para {existente.CantidadPersonas} persona(s).";
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("MisReservas", "Reservaciones");

        }



        [HttpPost("Eliminar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var reservacion = await _context.Reservaciones
                .FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == userId);

            if (reservacion == null)
                return Unauthorized();

            _context.Reservaciones.Remove(reservacion);
            await _context.SaveChangesAsync();

            return RedirectToAction("MisReservas", "Reservaciones");
        }

        [HttpGet("Disponibilidad")]
        public async Task<IActionResult> Disponibilidad(int habitacionId, DateTime inicio, DateTime fin, int? reservacionId = null, int? cantidadPersonas = null)
        {
            var fechaInicio = DateOnly.FromDateTime(inicio);
            var fechaFin = DateOnly.FromDateTime(fin);

            // 1️⃣ Buscar habitación
            var habitacion = await _context.Habitaciones.FindAsync(habitacionId);
            if (habitacion == null)
                return Json(new { disponible = false, mensaje = "La habitación no existe." });

            // 2️⃣ Validar capacidad de personas
            if (cantidadPersonas.HasValue && cantidadPersonas.Value > habitacion.Capacidad)
            {
                return Json(new
                {
                    disponible = false,
                    mensaje = $"⚠️ La habitación tiene una capacidad máxima de {habitacion.Capacidad} personas."
                });
            }

            // 3️⃣ Validar solapamiento de fechas
            bool existeSolapamiento = await _context.Reservaciones
                .Where(r => r.HabitacionId == habitacionId)
                .Where(r => reservacionId == null || r.Id != reservacionId) // Ignorar si está editando
                .AnyAsync(r =>
                    fechaInicio < r.FechaFin &&
                    fechaFin > r.FechaInicio
                );

            if (existeSolapamiento)
            {
                return Json(new
                {
                    disponible = false,
                    mensaje = "❌ La habitación ya está reservada en las fechas seleccionadas."
                });
            }

            // ✅ Todo bien
            return Json(new { disponible = true });
        }


        private async Task CargarHabitaciones(int? hotelId = null)
        {
            var habitaciones = await _context.Habitaciones
                .Where(h => h.Disponibilidad && (!hotelId.HasValue || h.HotelId == hotelId))
                .ToListAsync();

            ViewBag.Habitaciones = habitaciones;
            ViewBag.HotelId = hotelId;
        }

        [HttpGet("MisReservas")]
        [Authorize]
        public async Task<IActionResult> MisReservas()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var reservaciones = await _context.Reservaciones
                .Where(r => r.UsuarioId == userId)
                .Include(r => r.Habitacion)
                    .ThenInclude(h => h.Hotel)
                .Include(r => r.Facturaciones)
                .OrderByDescending(r => r.FechaReserva)
                .ToListAsync();

            return View("MisReservas", reservaciones);
        }

    }
}
