using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTasks
{
    public class Gestor : Utilizador
    {
        [Required]
        public string Departamento { get; set; }
        [Required]
        public string GereUtilizadores { get; set; }
    }
}
