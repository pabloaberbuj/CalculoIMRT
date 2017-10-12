using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_Independiente_IMRT
{
    public class Extraer
    {
        public static string[] cargar(string archivo)
        {
            try
            {
                return File.ReadAllLines(archivo);
            }
            catch (Exception e)
            {
                MessageBox.Show("No se ha podido abrir. Posiblemente el archivo esté en uso por otra aplicación\n\n" + e.ToString(), "Error abriendo el archivo");
                throw;
            }
        }

        public static double[] cargarMLC(string file)
        {
            string[] MLCfile = cargar(file);
            return extraerColumna(MLCfile, 1, 60);
        }

        public static string extraerString(string[] fid, int linea, char sep = '=')
        {
            string aux = fid[linea]; string[] aux2 = aux.Split(sep); string salida = aux2[1];
            return salida;
        }
        public static double extraerDouble(string[] fid, int linea)
        {
            return Convert.ToDouble(extraerString(fid, linea));
        }

        public static double[,] extraerMatriz(string[] fid, int lineaI, int lineaF, int columnas)
        {
            int filas = lineaF - lineaI + 1;
            double[,] M = new double[columnas, filas];
            for (int i = 0; i < filas; i++)
            {
                string[] stringLinea = fid[lineaI + i].Split('\t');
                double[] aux1 = stringLinea.Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                for (int j = 0; j < columnas; j++)
                {
                    M[j, i] = aux1[j];
                }
            }
            return M;
        }

        public static double[] extraerColumna(string[] fid, int lineaI, int lineaF)
        {
            int filas = lineaF - lineaI + 1;
            double[] M = new double[filas];
            for (int i = 0; i < filas; i++)
            {
                M[i] = Convert.ToDouble(fid[lineaI + i]);
            }
            return M;
        }

        public static int buscarSubStringEnFid(string[] fid, string SubString, int Inicio = 0)
        {
            int indice = 0;
            for (int i = Inicio; i < fid.Length; i++)
            {
                if (fid[i].Contains(SubString))
                {
                    indice = i;
                    break;
                }
            }
            return indice;
        }

        public static string buscarYExtraerString(string[] fid, string buscar, char sep = '=')
        {
            int linea = buscarSubStringEnFid(fid, buscar);
            return extraerString(fid, linea, sep);
        }

        public static string apellido(string[] fid)
        {
            return buscarYExtraerString(fid, "Last Name");
        }

        public static string nombre(string[] fid)
        {
            return buscarYExtraerString(fid, "First Name");
        }

        public static string hc(string[] fid)
        {
            return buscarYExtraerString(fid, "Patient ID");
        }

        public static int numSubCampos(string[] fid)
        {
            return Convert.ToInt32(buscarYExtraerString(fid, "Number of Fields"));
        }

        public static PuntoDeControl extraerPuntoDeControl(string[] fid, int indice)
        {
            int inicio = buscarSubStringEnFid(fid,"Field =",indice);
            PuntoDeControl pC = new PuntoDeControl()
            {
                nombre = extraerString(fid, inicio),
                posicionesA = new double[60],
                posicionesB = new double[60],
            };
            for (int i=0; i<60; i++)
            {
                pC.posicionesA[i] = extraerDouble(fid, inicio + i + 5);
                pC.posicionesB[i] = extraerDouble(fid, inicio + i + 65);
            }
            return pC;
        }

        public static Campo extraerCampo(string[] fid)
        {
            int indice = 0;
            Campo campo = new Campo()
            {
                numPC = numSubCampos(fid),
            };
            campo.puntosDeControl = new List<PuntoDeControl>();
            for (int i=0;i<campo.numPC;i++)
            {
                campo.puntosDeControl.Add(extraerPuntoDeControl(fid, indice));
                indice += 60;
            }
            return campo;
        }

    }
}
