using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PERFILES_SA.Models
{
    public class FiltroEmpleados
    {
        public int? DepartamentoId { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool? Activo { get; set; }
        public int Pagina { get; set; } = 1;
        public int RegistrosPorPagina { get; set; } = 20;
    }
}