using System;
using System.Collections.Generic;

namespace HotelCostaAzul.API.Models;

public partial class DetalleFactura
{
    public int Id { get; set; }

    public int IdFactura { get; set; }

    public string Descripcion { get; set; } = null!;

    public decimal PrecioUni { get; set; }

    public int DiasReserva { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Monto { get; set; }
}
