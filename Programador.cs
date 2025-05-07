using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTasks
{
    public enum Experiencia
    {
        Junior,
        Senior
    }
    class Programador : Utilizador
    {
        public Experiencia NivelExperiencia { get; set; }
        public string IdGestor { get; set; }
    }
}
