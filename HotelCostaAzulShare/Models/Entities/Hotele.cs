using System;
using System.Collections.Generic;

namespace HotelCostaAzul.API.Models;

public partial class Hotele
{
    public int Id { get; set; }

    public string NombreHotel { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Ciudad { get; set; } = null!;

    public string Pais { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal? Calificacion { get; set; }

    public string? ImagenHotel { get; set; }
}
