using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Respuesta
{
        public class ResultadoOperacion<TResult>
        {
            private ResultadoOperacion()
            {
            }
            public bool Exitoso { get; private set; }
            public TResult Resultado { get; private set; }
            public string MensajeProblema { get; private set; }
            public string MensajeInformacion { get; private set; }
            public Exception Excepcion { get; private set; }
            public string QueHacer { get; private set; }
           

        //Create a Success result without info messages
        public static ResultadoOperacion<TResult> CrearResultadoExitoso(TResult result)
        {
            return new ResultadoOperacion<TResult> { Exitoso = true, Resultado = result };
        }
        //Create a Success result with info messages
        public static ResultadoOperacion<TResult> CrearResultadoExitoso(TResult result, string info)
        {
            return new ResultadoOperacion<TResult> { Exitoso = true, Resultado = result, MensajeInformacion = info };
        }
        //Create a Failure result with simple message
        public static ResultadoOperacion<TResult> CrearFalla(string mensajeProblema)
        {
            return new ResultadoOperacion<TResult> { Exitoso = false, MensajeProblema = mensajeProblema };
        }
        //Create a Failure result with simple message and resolution message
        public static ResultadoOperacion<TResult> CrearFalla(string mensajeProblema, string intentar)
        {
            return new ResultadoOperacion<TResult> { Exitoso = false, MensajeProblema = mensajeProblema, QueHacer = intentar };
        }
        //Create a Failure result with exception message
        public static ResultadoOperacion<TResult> CrearFalla(Exception ex)
        {
            return new ResultadoOperacion<TResult>
            {
                Exitoso = false,
                MensajeProblema = ex.InnerException == null ? String.Format("{0}{1}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace) : String.Format("{0}{1}{1}{2}", ex.InnerException.Message, Environment.NewLine, ex.InnerException.StackTrace),
                Excepcion = ex.InnerException ?? ex
            };
        }



    }
}

