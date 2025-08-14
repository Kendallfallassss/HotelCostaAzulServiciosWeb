using System;
using System.Collections.Generic;

namespace HotelCostaAzul.API.Models;

public partial class Habitacione
{
    public int Id { get; set; }

    public int IdHotel { get; set; }

    public string Numero { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public decimal PrecioPersona { get; set; }

    public bool Estado { get; set; }

    public int Capacidad { get; set; }

    public string? Descripcion { get; set; }
}
