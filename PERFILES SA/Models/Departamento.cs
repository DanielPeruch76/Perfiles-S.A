using System;
namespace PERFILES_SA.Models
{
    public class Departamento
    {
        public int DepartamentoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public Departamento()
        {
            Activo = true;
            FechaCreacion = DateTime.Now;
        }
    }
}