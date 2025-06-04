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
            lstLista.SelectedIndex = -1;
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

        private void lstLista_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstLista.SelectedIndex == -1)
            {
                txtId.Clear();
                txtDesc.Clear();
                return;
            }
            if (lstLista.SelectedItem != null)
            {
                string selectedDescription = lstLista.SelectedItem.ToString();
                using (var db = new iTask())
                {
                    var tipoTarefa = db.TiposTarefas.FirstOrDefault(t => t.Descricao == selectedDescription);
                    if (tipoTarefa != null)
                    {
                        txtId.Text = tipoTarefa.Id.ToString();
                        txtDesc.Text = tipoTarefa.Descricao;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();

            foreach (Form form in Application.OpenForms)
            {
                if (form is frmKanban)
                {
                    form.Show();
                    break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (lstLista.SelectedItem == null)
            {
                MessageBox.Show("Selecione um Tipo de tarefa para remover!");
                return;
            }


            string descricao = lstLista.GetItemText(lstLista.SelectedItem);

            using (var db = new iTask())
            {
                var tipoParaRemover = db.TiposTarefas.FirstOrDefault(u => u.Descricao == descricao);

                if (tipoParaRemover != null)
                {
                    db.TiposTarefas.Remove(tipoParaRemover);
                    db.SaveChanges();

                    lstLista.DataSource = db.TiposTarefas.ToList();
                    lstLista.DisplayMember = "Descricao";

                    MessageBox.Show("Tipo de tarefa removido com sucesso!");
                }
                else
                {
                    MessageBox.Show("Tipo de tarefa não encontrado!");
                }
            }
        }
    }
}
