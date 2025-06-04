using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTasks
{
    public class TipoTarefa
    {
        public int Id { get; set; }
        public string Descricao { get; set; }

        public virtual ICollection<Tarefa> Tarefas { get; set; }
    }
}
