using System;
using System.Collections.Generic;

namespace HotelCostaAzul.API.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string NombreUsu { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int IdRol { get; set; }

    public DateTime FechaRegistro { get; set; }
}
