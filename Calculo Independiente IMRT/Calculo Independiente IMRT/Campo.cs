using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_Independiente_IMRT
{
    public class Campo
    {
        public int numPC { get; set; }
        public List<PuntoDeControl> puntosDeControl {get;set;}
        public double [,] mapaFluencia { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
