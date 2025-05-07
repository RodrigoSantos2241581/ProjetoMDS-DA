using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTasks
{
    public enum Departamento
    {
        IT,
        Marketing,
        Administracao
    }
    class Gestor : Utilizador
    {
        public Departamento Departamento { get; set; }
        public string GereUtilizadores { get; set; }
    }
}
