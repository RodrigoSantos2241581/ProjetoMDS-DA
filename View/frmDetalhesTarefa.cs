using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iTasks
{

    public partial class frmDetalhesTarefa : Form
    {
        public frmDetalhesTarefa(int id)
        {
            InitializeComponent();
            CarregarProgramadores(id);
            CarregarTipoTarefa();
            MostrarDetalhes(id);
        }
        private void MostrarDetalhes(int id)
        {
            //mostrar os detalhes da tarefa selecionada
            using (var db = new iTask())
            {
                var tarefa = db.Tarefas
                    .Include(t => t.Programador)
                    .Include(t => t.Gestor)
                    .Include(t => t.TipoTarefa)
                    .FirstOrDefault(t => t.Id == id);
                if (tarefa != null)
                {
                    txtId.Text = tarefa.Id.ToString();
                    txtDataRealini.Text = tarefa.DataPrevistaInicio.ToString("dd/MM/yyyy");
                    txtDataRealFim.Text = tarefa.DataPrevistaFim.ToString("dd/MM/yyyy");
                    txtEstado.Text = tarefa.EstadoAtual;
                    txtDataCriacao.Text = tarefa.DataCriacao.ToString("dd/MM/yyyy");

                    txtDesc.Text = tarefa.Descricao;
                    txtOrdem.Text = tarefa.Ordem;
                    txtStoryPoints.Text = tarefa.StoryPoints.ToString();
                    dtInicio.Value = tarefa.DataPrevistaInicio;
                    dtFim.Value = tarefa.DataPrevistaFim;
                    if (tarefa.Programador != null)
                    {
                        cbProgramador.SelectedItem = tarefa.Programador.Username;
                    }
                    if (tarefa.TipoTarefa != null)
                    {
                        cbTipoTarefa.SelectedItem = tarefa.TipoTarefa.Descricao;
                    }
                }
            }
        }

        private void CarregarProgramadores(int gestorId)
        {
            try
            {
                cbProgramador.Items.Clear();
                cbProgramador.Text = "Selecione um Programador";

                // Correção: Usar a instância correta do contexto de dados
                using (var context = new iTask()) // Substitua "iTask" pelo nome real da sua classe de contexto
                {
                    var programadores = context.Utilizadores
                                       .OfType<Programador>()
                                       .Where(p => p.Gestor != null &&
                                                  p.Gestor.Id == gestorId &&
                                                  !string.IsNullOrEmpty(p.Username))
                                       .OrderBy(p => p.Username)
                                       .ToList();

                    if (programadores.Any())
                    {
                        foreach (var programador in programadores)
                        {
                            cbProgramador.Items.Add(
                                programador.Username
                            );
                        }

                        cbProgramador.SelectedIndex = 0;
                    }
                    else
                    {
                        cbProgramador.Items.Add("Nenhum programador disponível");
                        cbProgramador.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar programadores: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbProgramador.Items.Add("Erro ao carregar");
                cbProgramador.Enabled = false;
            }
        }

        private void CarregarTipoTarefa()
        {
            try
            {
                cbTipoTarefa.Items.Clear();
                cbTipoTarefa.Text = "Selecione um Tipo de Tarefa";
                using (var context = new iTask())
                {
                    var tiposTarefa = context.TiposTarefas
                                             .Select(t => t.Descricao)
                                             .Distinct()
                                             .OrderBy(t => t)
                                             .ToList();
                    if (tiposTarefa.Any())
                    {
                        foreach (var tipo in tiposTarefa)
                        {
                            cbTipoTarefa.Items.Add(tipo);
                        }
                        cbTipoTarefa.SelectedIndex = 0;
                    }
                    else
                    {
                        cbTipoTarefa.Items.Add("Nenhum tipo de tarefa disponível");
                        cbTipoTarefa.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar tipos de tarefa: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbTipoTarefa.Items.Add("Erro ao carregar");
                cbTipoTarefa.Enabled = false;
            }
        }
        private void btGravar_Click(object sender, EventArgs e)
        {
            string descricao = txtDesc.Text;
            string programador = cbProgramador.SelectedItem?.ToString();
            string tipoTarefa = cbTipoTarefa.SelectedItem?.ToString();
            try
            {
                if (string.IsNullOrWhiteSpace(descricao))
                {
                    MessageBox.Show("A descrição não pode estar vazia.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(programador) || programador == "Selecione um Programador")
                {
                    MessageBox.Show("Por favor, selecione um programador.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(tipoTarefa) || tipoTarefa == "Selecione um Tipo de Tarefa")
                {
                    MessageBox.Show("Por favor, selecione um tipo de tarefa.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                AdicionarTarefa(descricao, programador, tipoTarefa);
                MessageBox.Show("Tarefa adicionada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtDesc.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar tarefa: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void AdicionarTarefa(string descricao, string programador, string tipoTarefa)
        {
            using (var db = new iTask())
            {
                try
                {
                    // 1. Obter as entidades relacionadas
                    var programadorEntity = db.Utilizadores
                        .OfType<Programador>()
                        .Include(p => p.Gestor)  // Carrega o gestor relacionado
                        .FirstOrDefault(p => p.Username == programador);

                    var tipoTarefaEntity = db.TiposTarefas
                        .FirstOrDefault(t => t.Descricao == tipoTarefa);

                    // 2. Validações
                    if (programadorEntity == null)
                    {
                        MessageBox.Show("Programador não encontrado", "Erro",
                                     MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (tipoTarefaEntity == null)
                    {
                        MessageBox.Show("Tipo de tarefa não encontrado", "Erro",
                                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (programadorEntity.Gestor == null)
                    {
                        MessageBox.Show("O programador não tem um gestor associado", "Erro",
                                     MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 3. Criar a nova tarefa
                    var tarefa = new Tarefa
                    {
                        Descricao = descricao,
                        IdProgramador = programadorEntity.Id,
                        Programador = programadorEntity,
                        IdGestor = programadorEntity.Gestor.Id,
                        Gestor = programadorEntity.Gestor,
                        IdTipoTarefa = tipoTarefaEntity.Id,
                        TipoTarefa = tipoTarefaEntity,
                        DataCriacao = DateTime.Now,
                        Ordem = txtOrdem.Text,
                        StoryPoints = !string.IsNullOrWhiteSpace(txtStoryPoints.Text) ?
                                     int.Parse(txtStoryPoints.Text) : 1,
                        DataPrevistaInicio = dtInicio.Value,
                        DataPrevistaFim = dtFim.Value,
                        EstadoAtual = "To Do"
                    };

                    // 4. Adicionar e salvar
                    db.Tarefas.Add(tarefa);
                    db.SaveChanges();

                    MessageBox.Show("Tarefa criada com sucesso!", "Sucesso",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (DbUpdateException dbEx)
                {
                    // Tratar erros específicos do banco de dados
                    var errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                    MessageBox.Show($"Erro ao salvar no banco de dados: {errorMessage}", "Erro",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro: {ex.Message}", "Erro",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btFechar_Click(object sender, EventArgs e)
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
    }
}
