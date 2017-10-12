using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculo_Independiente_IMRT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Archivos mlc(.mlc)|*.mlc|All Files (*.*)|*.*";
            openFileDialog1.ShowDialog();
            string[] fid = Extraer.cargar(openFileDialog1.FileName);
            Campo campo = Extraer.extraerCampo(fid);
            Campo campo2 = Calcular.agregarPuntosDeControl(campo, 5);
            double[] mlc = Extraer.cargarMLC(Configuracion.fileMLC);
          double[,] fluencia = Calcular.fluenciaCampo(campo2, Configuracion.tamMatriz, Configuracion.numPuntos, Configuracion.factorTransmision, mlc);
            using (var sw = new StreamWriter("fluencia.txt"))
            {
                for (int i = 0; i < fluencia.GetLength(0); i++)
                {
                    for (int j = 0; j < fluencia.GetLength(1); j++)
                    {
                        sw.Write(fluencia[j, i] + " ");
                    }
                    sw.Write("\n");
                }

                sw.Flush();
                sw.Close();
            }

            double[] vectorRadios = Calcular.vectorRadios(0, 0, Configuracion.tamMatriz, Configuracion.numPuntos);
            double[] fluenciaRadio = Calcular.vectorFluenciaRadios(0, 0, Configuracion.tamMatriz, Configuracion.numPuntos, fluencia);
            using (StreamWriter sr = new StreamWriter("radioFluencia.txt"))
            {
                for (int i = 0; i < vectorRadios.Count(); i++)
                {
                    string linea = vectorRadios[i].ToString() + "\t" + fluenciaRadio[i].ToString();
                    sr.WriteLine(linea);
                }
            }
            double max = fluencia.Cast<double>().Max();
            MessageBox.Show(max.ToString());
        }
    }
}
