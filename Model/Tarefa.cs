using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTasks
{
    public class Tarefa
    {
        public int Id { get; set; }

        [Required]
        public int IdGestor { get; set; }

        [ForeignKey("IdGestor")]
        public virtual Gestor Gestor { get; set; }

        [Required]
        public int IdProgramador { get; set; }

        [ForeignKey("IdProgramador")]
        public virtual Programador Programador { get; set; }

        public string Ordem { get; set; }
        public string Descricao { get; set; }
        public DateTime DataPrevistaInicio { get; set; }
        public DateTime DataPrevistaFim { get; set; }

        [Required]
        public int IdTipoTarefa { get; set; }  // Alterado para int (chave estrangeira)

        [ForeignKey("IdTipoTarefa")]
        public virtual TipoTarefa TipoTarefa { get; set; }  // Propriedade de navegação

        public int StoryPoints { get; set; }
        public DateTime? DataRealInicio { get; set; }  // Tornado nullable
        public DateTime? DataRealFim { get; set; }    // Tornado nullable
        public DateTime DataCriacao { get; set; }
        public string EstadoAtual { get; set; }
    }
}
