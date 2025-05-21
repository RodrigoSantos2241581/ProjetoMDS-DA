using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTasks
{
    public class Programador : Utilizador
    {
        [Required]
        public string NivelExperiencia { get; set; }

        public Gestor Gestor { get; set; }
    }
}
