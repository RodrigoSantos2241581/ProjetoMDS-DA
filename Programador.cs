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
        [Required]
        public string IdGestor { get; set; }
    }
}
