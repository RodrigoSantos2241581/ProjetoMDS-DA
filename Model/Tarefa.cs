using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTasks
{
    class Tarefa
    {
        public int Id { get; set; }

        [Required]
        Gestor IdGestor { get; set; }

        [Required]
        Programador IdProgramador { get; set; }

        public string OrdemExecucao { get; set; }
        public string Descricao { get; set; }
        public DateTime DataPrevistaInicio { get; set; }
        public DateTime DataPrevistaFim { get; set; }

        [Required]
        public TipoTarefa IdTipoTarefa { get; set; }

        public int StoryPoints { get; set; }
        public DateTime DataRealInicio { get; set; }
        public DateTime DataRealFim { get; set; }
        public DateTime DataCriacao { get; set; }
        public string EstadoAtual { get; set; }

    }
}
