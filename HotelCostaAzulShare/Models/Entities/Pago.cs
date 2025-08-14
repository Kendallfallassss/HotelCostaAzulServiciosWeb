using System;
using System.Collections.Generic;

namespace HotelCostaAzul.API.Models;

public partial class Pago
{
    public int Id { get; set; }

    public int IdReserva { get; set; }

    public int IdMetodoPago { get; set; }

    public decimal MontoTotal { get; set; }

    public int? IdFacturacion { get; set; }

    public DateTime FechaPago { get; set; }
}
