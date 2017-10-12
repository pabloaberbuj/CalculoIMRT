using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_Independiente_IMRT
{
    public class Plan
    {
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string HC { get; set; }
        public List<Campo> campos { get; set; }
    }
}
