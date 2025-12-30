namespace PERFILES_SA.Models
{
    public class RespuestaOperacion
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public object Data { get; set; }

        public RespuestaOperacion()
        {
            Exitoso = true;
            Mensaje = "Operación completada exitosamente";
        }

        public static RespuestaOperacion Error(string mensaje)
        {
            return new RespuestaOperacion
            {
                Exitoso = false,
                Mensaje = mensaje
            };
        }

        public static RespuestaOperacion Exito(object data = null, string mensaje = null)
        {
            return new RespuestaOperacion
            {
                Exitoso = true,
                Mensaje = mensaje ?? "Operación completada exitosamente",
                Data = data
            };
        }
    }
}