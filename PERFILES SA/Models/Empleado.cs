using System;

namespace PERFILES_SA.Models
{
    public class Empleado
    {
        public int EmpleadoId { get; set; }
        public string DPI { get; set; }
        public string Nombres { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Sexo { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Direccion { get; set; }
        public string NIT { get; set; }
        public int DepartamentoId { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public int Edad
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - FechaNacimiento.Year;
                if (FechaNacimiento.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        public int AniosLaborando
        {
            get
            {
                var today = DateTime.Today;
                var years = today.Year - FechaIngreso.Year;
                if (FechaIngreso.Date > today.AddYears(-years)) years--;
                return years;
            }
        }

        public int MesesLaborando
        {
            get
            {
                var today = DateTime.Today;
                var months = ((today.Year - FechaIngreso.Year) * 12) + today.Month - FechaIngreso.Month;
                if (today.Day < FechaIngreso.Day) months--;
                return months % 12;
            }
        }

        public string NombreDepartamento { get; set; }
        public bool DepartamentoActivo { get; set; }

        public Empleado()
        {
            Activo = true;
            FechaCreacion = DateTime.Now;
        }
    }
}