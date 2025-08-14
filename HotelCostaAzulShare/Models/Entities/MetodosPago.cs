using System;
using System.Collections.Generic;

namespace HotelCostaAzul.API.Models;

public partial class MetodosPago
{
    public int Id { get; set; }

    public string Tipo { get; set; } = null!;

    public string? IdUsuario { get; set; }
}
