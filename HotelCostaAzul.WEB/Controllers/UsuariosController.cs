using HotelCostaAzul.API.Models.Entities;
using HotelCostaAzul.API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using HotelCostaAzul.API.Services;
using HotelCostaAzul.API.Models;

public class UsuariosController : Controller
{
    private readonly HotelifyContext _context;
    private readonly HashingService _hashing;

    public UsuariosController(HotelifyContext context, HashingService hashing)
    {
        _context = context;
        _hashing = hashing;
    }

    // GET: /Usuarios/Auth
    public IActionResult Auth()
    {
        CargarRoles();
        return View();
    }


    private void CargarRoles()
    {
        ViewBag.Roles = new SelectList(_context.Roles.ToList(), "Id", "Tipo");
    }

    // GET: /Usuarios/Login
    public IActionResult Login() => RedirectToAction("Auth");

    [HttpPost]
    public async Task<IActionResult> Login(string correo, string password)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo);

        if (usuario != null && _hashing.VerifyPassword(password, usuario.PasswordHash))
        {
            var rol = _context.Roles.FirstOrDefault(r => r.Id == usuario.RolId);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.NombreUsuario),
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()), // ✅ ESTO ES CLAVE
            new Claim("UsuarioId", usuario.Id.ToString()), // ✅ Opcional si lo usas en otras partes
            new Claim(ClaimTypes.Role, rol?.Tipo ?? "Cliente")
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // 🔀 Redirección por tipo de rol
            return rol?.Tipo == "Administrador"
                ? RedirectToAction("Dashboard", "Admin")
                : RedirectToAction("Dashboard", "Cliente");
        }

        // ❌ Si no pasa la verificación:
        ModelState.AddModelError(string.Empty, "Correo o contraseña inválidos.");
        ViewBag.LoginFailed = true;
        ViewBag.Roles = new SelectList(_context.Roles.ToList(), "Id", "Tipo");
        return View("Auth");
    }



    // GET: /Usuarios/Registro
    public IActionResult Registro()
    {
        CargarRoles();
        return View();
    }

    [HttpPost]
    public IActionResult Registro(Usuario nuevoUsuario)
    {
        ViewBag.Roles = new SelectList(_context.Roles.ToList(), "Id", "Tipo");

        if (ModelState.IsValid)
        {
            // ✅ Validar correo duplicado
            if (_context.Usuarios.Any(u => u.Correo == nuevoUsuario.Correo))
            {
                ModelState.AddModelError("Correo", "El correo ya está registrado.");
                return View(nuevoUsuario);
            }

            // ✅ Hashear la contraseña antes de guardar
            nuevoUsuario.PasswordHash = _hashing.HashPassword(nuevoUsuario.PasswordHash);

            // Fechas de auditoría
            nuevoUsuario.CreatedAt = DateTime.UtcNow;
            nuevoUsuario.FechaRegistro = DateTime.UtcNow;

            _context.Usuarios.Add(nuevoUsuario);
            _context.SaveChanges();

            // ✅ Redirige a login después del registro
            return RedirectToAction("Auth");
        }

        return View(nuevoUsuario);
    }


    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["LogoutSuccess"] = "Sesión cerrada correctamente.";
        return RedirectToAction("Auth", "Usuarios");
    }
}
