using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccesoDatos
{
        public class DataOperacion<TResult>
        {
            private DataOperacion()
            {
            }
            public bool Exitoso { get; private set; }
            public TResult Resultado { get; private set; }
            public string MensajeProblema { get; private set; }
            public string MensajeInformacion { get; private set; }
            public Exception Excepcion { get; private set; }
            public string QueHacer { get; private set; }
           

        //Create a Success result without info messages
        public static DataOperacion<TResult> CrearResultadoExitoso(TResult result)
        {
            return new DataOperacion<TResult> { Exitoso = true, Resultado = result };
        }
        //Create a Success result with info messages
        public static DataOperacion<TResult> CrearResultadoExitoso(TResult result, string info)
        {
            return new DataOperacion<TResult> { Exitoso = true, Resultado = result, MensajeInformacion = info };
        }
        //Create a Failure result with simple message
        public static DataOperacion<TResult> CrearFalla(string mensajeProblema)
        {
            return new DataOperacion<TResult> { Exitoso = false, MensajeProblema = mensajeProblema };
        }
        //Create a Failure result with simple message and resolution message
        public static DataOperacion<TResult> CrearFalla(string mensajeProblema, string intentar)
        {
            return new DataOperacion<TResult> { Exitoso = false, MensajeProblema = mensajeProblema, QueHacer = intentar };
        }
        //Create a Failure result with exception message
        public static DataOperacion<TResult> CrearFalla(Exception ex)
        {
            return new DataOperacion<TResult>
            {
                Exitoso = false,
                MensajeProblema = ex.InnerException == null ? String.Format("{0}{1}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace) : String.Format("{0}{1}{1}{2}", ex.InnerException.Message, Environment.NewLine, ex.InnerException.StackTrace),
                Excepcion = ex.InnerException ?? ex
            };
        }






    }
}


