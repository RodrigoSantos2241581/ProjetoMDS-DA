using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iTasks
{
    public partial class frmGereTiposTarefas : Form
    {
        public frmGereTiposTarefas()
        {
            InitializeComponent();
            LerBaseDados();
        }

        public void LerBaseDados()
        {
            using (var db = new iTask())
            {
                lstLista.DataSource = db.TiposTarefas.OfType<TipoTarefa>()
                                             .Select(g => g.Descricao)
                                             .Distinct()
                                             .ToList();
            }
        }
        private void btGravar_Click(object sender, EventArgs e)
        {
            string descricao = txtDesc.Text;

            try
            {
                if (string.IsNullOrWhiteSpace(descricao))
                {
                    MessageBox.Show("A descrição não pode estar vazia.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                AdicionarTipoTarefa(descricao);
                MessageBox.Show("Tipo de tarefa adicionado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtDesc.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar tipo de tarefa: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    

    public void AdicionarTipoTarefa(string descricao)
        {
            using (var db = new iTask())
            {
                if (!db.TiposTarefas.Any(u => u.Descricao == descricao))
                {
                    var TipoTarefa = new TipoTarefa
                    {
                        Descricao = descricao
                    };

                    db.TiposTarefas.Add(TipoTarefa);
                    db.SaveChanges();
                }
            }
        }
    }
}
