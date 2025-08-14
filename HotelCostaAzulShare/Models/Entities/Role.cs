using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelCostaAzul.API.Models.Entities
{
    public class Roles
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [RegularExpression("^(Administrador|Cliente)$", ErrorMessage = "El nombre debe ser 'Admin' o 'User'.")]
        public string Tipo { get; set; }

        public string? Descripcion { get; set; }
    }
}
