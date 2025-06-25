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
                lstLista.DataSource = db.TiposTarefas
                                      .Select(t => t.Descricao)
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
                    MessageBox.Show("A descrição não pode estar vazia.", "Erro",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var db = new iTask())
                {
                    if (!string.IsNullOrWhiteSpace(txtId.Text)) // Modo edição
                    {
                        int id = int.Parse(txtId.Text);
                        var tipoExistente = db.TiposTarefas.Find(id);

                        if (tipoExistente != null)
                        {
                            // Verifica se a descrição foi alterada para um valor que já existe
                            if (tipoExistente.Descricao != descricao &&
                                db.TiposTarefas.Any(t => t.Descricao == descricao))
                            {
                                MessageBox.Show("Já existe um tipo de tarefa com esta descrição.", "Erro",
                                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            tipoExistente.Descricao = descricao;
                            db.SaveChanges();
                            MessageBox.Show("Tipo de tarefa atualizado com sucesso!", "Sucesso",
                                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else // Modo criação
                    {
                        if (db.TiposTarefas.Any(t => t.Descricao == descricao))
                        {
                            MessageBox.Show("Já existe um tipo de tarefa com esta descrição.", "Erro",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        var novoTipo = new TipoTarefa { Descricao = descricao };
                        db.TiposTarefas.Add(novoTipo);
                        db.SaveChanges();
                        MessageBox.Show("Tipo de tarefa adicionado com sucesso!", "Sucesso",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                txtDesc.Clear();
                txtId.Clear();
                LerBaseDados();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            string descricaoSelecionada = lstLista.SelectedItem.ToString();

            using (var db = new iTask())
            {
                var tipo = db.TiposTarefas.FirstOrDefault(t => t.Descricao == descricaoSelecionada);
                if (tipo != null)
                {
                    txtId.Text = tipo.Id.ToString();
                    txtDesc.Text = tipo.Descricao;
                }
            }
        }
        private void btVoltar_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            var kanbanForm = Application.OpenForms.OfType<frmKanban>().FirstOrDefault();
            kanbanForm?.Show();
        }

        private void btRemover_Click_1(object sender, EventArgs e)
        {
            if (lstLista.SelectedItem == null)
            {
                MessageBox.Show("Selecione um tipo de tarefa para remover!", "Aviso",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string descricao = lstLista.SelectedItem.ToString();

            try
            {
                using (var db = new iTask())
                {
                    var tipoParaRemover = db.TiposTarefas.FirstOrDefault(t => t.Descricao == descricao);

                    if (tipoParaRemover != null)
                    {
                        // Verifica se existem tarefas associadas a este tipo
                        if (tipoParaRemover.Tarefas != null && tipoParaRemover.Tarefas.Any())
                        {
                            MessageBox.Show("Não é possível remover este tipo de tarefa pois existem tarefas associadas a ele.",
                                          "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        db.TiposTarefas.Remove(tipoParaRemover);
                        db.SaveChanges();

                        MessageBox.Show("Tipo de tarefa removido com sucesso!", "Sucesso",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LerBaseDados();
                        txtId.Clear();
                        txtDesc.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao remover: {ex.Message}", "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}