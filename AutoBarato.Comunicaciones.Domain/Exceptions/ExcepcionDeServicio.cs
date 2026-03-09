using  System;

namespace AutoBarato.Comunicaciones.Domain.Exceptions
{
    public class ExcepcionDeServicio : Exception
    {
        public ExcepcionDeServicio() : base() { }

        public ExcepcionDeServicio(string message) : base(message) { }

        public ExcepcionDeServicio(string message, Exception innerException) : base(message, innerException) { }
    }
}