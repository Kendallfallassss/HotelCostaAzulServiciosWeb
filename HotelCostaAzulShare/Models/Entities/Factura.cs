using System;
using System.Collections.Generic;

namespace HotelCostaAzul.API.Models;

public partial class Factura
{
    public int Id { get; set; }

    public string NumeroFactura { get; set; } = null!;

    public int IdReserva { get; set; }

    public decimal MontoTotal { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime Fecha { get; set; }
}
