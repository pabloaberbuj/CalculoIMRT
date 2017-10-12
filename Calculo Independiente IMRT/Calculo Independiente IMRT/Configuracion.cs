using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_Independiente_IMRT
{
    public static class Configuracion
    {
        public static double tamMatriz = 40;//tamaño matriz en cm
        public static int numPuntos = 400; //número de puntos en cada linea
        public static double factorTransmision = 0.03; //factor de transmisión de una lámina
        public static string fileMLC = @"MLC_millenium.txt";
    }
}
