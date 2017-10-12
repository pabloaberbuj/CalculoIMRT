using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Calculo_Independiente_IMRT
{
    public class Calcular
    {

        public static double posicionIndice(int indice, double tamMatriz, int numPuntos)
        {
            return indice * tamMatriz / numPuntos - tamMatriz / 2;
        }

        public static int laminaIndice(int indice, double tamMatriz, int numPuntos, double[] MLCarray)
        {
            double posicion = posicionIndice(indice, tamMatriz, numPuntos);
            int lamina = Array.FindIndex(MLCarray, x => x >= posicion); //busca la lámina que empieza después de ese punto (-1 porque quiero la anterior +1 porque la lámina 1 tiene index = 0)
            if (lamina == -1)
            {
                lamina = MLCarray.Count() - 1;
            }
            return lamina;
        }

        public static double distancia(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }
        public static double distanciaPuntoPixel(double x1, double y1, int indiceX2, int indiceY2, double tamMatriz, int numPuntos)
        {
            double x2 = posicionIndice(indiceX2, tamMatriz, numPuntos);
            double y2 = posicionIndice(indiceY2, tamMatriz, numPuntos);

            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        public static double distanciaEntrePixeles(int indiceX1, int indiceY1, int indiceX2, int indiceY2, double tamMatriz, int numPuntos)
        {
            double x1 = posicionIndice(indiceX1, tamMatriz, numPuntos);
            double x2 = posicionIndice(indiceX2, tamMatriz, numPuntos);
            double y1 = posicionIndice(indiceY1, tamMatriz, numPuntos);
            double y2 = posicionIndice(indiceY2, tamMatriz, numPuntos);

            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        public static bool estaLibrePC(int indice, int lamina, PuntoDeControl pC, double tamMatriz, int numPuntos)
        {
            return (posicionIndice(indice, tamMatriz, numPuntos) > -pC.posicionesB[lamina] && posicionIndice(indice, tamMatriz, numPuntos) < pC.posicionesA[lamina]);
        }

        public static double fluenciaIndiceLaminaPC(int indice, int lamina, PuntoDeControl pC, double tamMatriz, int numPuntos, double factorTransmision)
        {
            if (estaLibrePC(indice, lamina, pC, tamMatriz, numPuntos))
            {
                return 1;
            }
            else
            {
                return factorTransmision;
            }
        }
        public static double fluenciaIndiceLaminaCampo(int indice, int lamina, Campo campo, double tamMatriz, int numPuntos, double factorTransmision)
        {
            double fluenciaSinNormaliz = 0;
            foreach (PuntoDeControl pC in campo.puntosDeControl)
            {
                fluenciaSinNormaliz += fluenciaIndiceLaminaPC(indice, lamina, pC, tamMatriz, numPuntos, factorTransmision);
            }
            return fluenciaSinNormaliz / campo.numPC;
        }

        public static double fluenciaPunto(int indiceX, int indiceY, Campo campo, double tamMatriz, int numPuntos, double factorTransmision, double[] MLCarray)
        {
            int lamina = laminaIndice(indiceY, tamMatriz, numPuntos, MLCarray);
            return fluenciaIndiceLaminaCampo(indiceX, lamina, campo, tamMatriz, numPuntos, factorTransmision);
        }

        public static double[,] fluenciaCampo(Campo campo, double tamMatriz, int numPuntos, double factorTransmision, double[] MLCarray)
        {
            double[,] fluencia = new double[numPuntos, numPuntos];
            for (int i = 0; i < numPuntos; i++)
            {
                for (int j = 0; j < numPuntos; j++)
                {
                    fluencia[i, j] = fluenciaPunto(i, j, campo, tamMatriz, numPuntos, factorTransmision, MLCarray);
                }
            }
            return fluencia;
        }

        public static double[] vectorRadios(double x0, double y0, double tamMatriz, int numPuntos)
        {
            double radioMax = Math.Sqrt((tamMatriz / 2 + Math.Abs(x0)) * (tamMatriz / 2 + Math.Abs(x0)) + (tamMatriz / 2 + Math.Abs(y0)) * (tamMatriz / 2 + Math.Abs(y0)));
            double[] radios = new double[(int)Math.Ceiling(radioMax * numPuntos / tamMatriz)];
            for (int i = 0; i < radios.Count(); i++)
            {
                radios[i] = (i + 1) * tamMatriz / numPuntos;
            }
            return radios;
        }

        public static double[] vectorFluenciaRadios(double x0, double y0, double tamMatriz, int numPuntos, double[,] fluencia)
        {
            double[] radios = vectorRadios(x0, y0, tamMatriz, numPuntos);
            double[] puntosPorRadio = new double[radios.Count()];
            double[] fluenciaRadiosSN = new double[radios.Count()];
            double[] fluenciaRadios = new double[radios.Count()];
            for (int i = 0; i < fluencia.GetLength(0); i++)
            {
                for (int j = 0; j < fluencia.GetLength(1); j++)
                {
                    double dist = distanciaPuntoPixel(x0, y0, i, j, tamMatriz, numPuntos);
                    int indiceRadios = Array.FindIndex(radios, r => r > dist);
                    fluenciaRadiosSN[indiceRadios] += fluencia[i, j];
                    puntosPorRadio[indiceRadios]++;
                }
            }
            for (int i = 0; i < fluenciaRadios.Count(); i++)
            {
                fluenciaRadios[i] = fluenciaRadiosSN[i] / puntosPorRadio[i];
            }
            return fluenciaRadios;
        }

        public static PuntoDeControl pCIntermedio(PuntoDeControl pC1, PuntoDeControl pC2, int indice, int totalPCagregados)
        {
            PuntoDeControl pC = new PuntoDeControl();
            {
                pC.posicionesA = new double[60];
                pC.posicionesB = new double[60];
            }
            for (int i = 0; i < 60; i++)
            {
                pC.posicionesA[i] = pC1.posicionesA[i] + (pC2.posicionesA[i] - pC1.posicionesA[i]) * indice / totalPCagregados;
                pC.posicionesB[i] = pC1.posicionesB[i] + (pC2.posicionesB[i] - pC1.posicionesB[i]) * indice / totalPCagregados;
            }
            return pC;
        }

        public static Campo agregarPuntosDeControl(Campo campo, int puntosAAgregar)
        {
            Campo campoExt = (Campo)campo.Clone();
            for (int i=0;i<campo.numPC-1;i++)
            {
                for (int j=1;j<puntosAAgregar;j++)
                {
                    campo.puntosDeControl.Add(pCIntermedio(campo.puntosDeControl[i], campo.puntosDeControl[i + 1], j, puntosAAgregar));
                }
            }
            campoExt.numPC = campo.puntosDeControl.Count();
            return campoExt;
        }
    }
}
