using System;
using System.Collections.Generic;

namespace HotelCostaAzul.API.Models;

public partial class Reserva
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    public int IdHabitacion { get; set; }

    public DateOnly FechaIngreso { get; set; }

    public DateOnly FechaSalida { get; set; }

    public DateTime FechaReserva { get; set; }

    public int CantidadPersonas { get; set; }

    public decimal Monto { get; set; }

    public int? IdHotel { get; set; }
}
