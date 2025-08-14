using System;
using System.Collections.Generic;

namespace HotelCostaAzul.API.Models;

public partial class Comentario
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    public int IdHotel { get; set; }

    public decimal Calificacion { get; set; }

    public string? Comentario1 { get; set; }

    public DateTime Fecha { get; set; }
}
