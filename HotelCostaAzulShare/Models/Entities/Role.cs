using System;
using System.Collections.Generic;

namespace HotelCostaAzul.API.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Tipo { get; set; } = null!;

    public string? Descripcion { get; set; }
}
