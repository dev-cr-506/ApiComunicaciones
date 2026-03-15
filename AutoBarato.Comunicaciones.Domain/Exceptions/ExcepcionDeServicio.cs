
namespace AutoBarato.Comunicaciones.Domain.Exceptions
{
    public class ExcepcionDeServicio : Exception
    {
        public ExcepcionDeServicio() : base() { }

        public ExcepcionDeServicio(string mensaje) : base(mensaje) { }

        public ExcepcionDeServicio(string mensaje, Exception excepcionInterna) : base(mensaje, excepcionInterna) { }
    }
}