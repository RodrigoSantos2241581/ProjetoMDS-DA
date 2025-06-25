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
        public frmDetalhesTarefa(int gestorId, int? tarefaId = null)
        {
            InitializeComponent();
            CarregarProgramadores(gestorId);
            CarregarTipoTarefa();

            // Só mostra detalhes se for uma tarefa existente
            if (tarefaId.HasValue && tarefaId.Value > 0)
            {
                MostrarDetalhes(tarefaId.Value);
                this.Text = "Editar Tarefa";
            }
            else
            {
                this.Text = "Nova Tarefa";
                // Configura valores padrão para nova tarefa
                txtEstado.Text = "To Do";
                txtDataCriacao.Text = DateTime.Now.ToString("dd/MM/yyyy");
                dtInicio.Value = DateTime.Now;
                dtFim.Value = DateTime.Now.AddDays(1);
            }
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

                using (var db = new iTask())
                {
                    // Consulta corrigida para buscar todos programadores do gestor
                    var programadores = db.Utilizadores
                                       .OfType<Programador>()
                                       .Where(p => p.Gestor.Id == gestorId)
                                       .OrderBy(p => p.Username)
                                       .ToList();

                    if (programadores.Any())
                    {
                        foreach (var programador in programadores)
                        {
                            cbProgramador.Items.Add(programador.Username);
                        }

                        // Não seleciona automaticamente o primeiro item
                        cbProgramador.SelectedIndex = -1;
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

                // Verifica se é uma edição (txtId preenchido) ou nova tarefa
                if (!string.IsNullOrWhiteSpace(txtId.Text) && int.TryParse(txtId.Text, out int tarefaId))
                {
                    AtualizarTarefa(tarefaId, descricao, programador, tipoTarefa);
                    MessageBox.Show("Tarefa atualizada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    AdicionarTarefa(descricao, programador, tipoTarefa);
                    MessageBox.Show("Tarefa adicionada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtDesc.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao processar tarefa: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarTarefa(int tarefaId, string descricao, string programador, string tipoTarefa)
        {
            using (var db = new iTask())
            {
                try
                {
                    // 1. Obter a tarefa existente
                    var tarefa = db.Tarefas
                        .Include(t => t.Programador)
                        .Include(t => t.TipoTarefa)
                        .FirstOrDefault(t => t.Id == tarefaId);

                    if (tarefa == null)
                    {
                        MessageBox.Show("Tarefa não encontrada", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 2. Obter as entidades relacionadas
                    var programadorEntity = db.Utilizadores
                        .OfType<Programador>()
                        .FirstOrDefault(p => p.Username == programador);

                    var tipoTarefaEntity = db.TiposTarefas
                        .FirstOrDefault(t => t.Descricao == tipoTarefa);

                    // 3. Validações
                    if (programadorEntity == null)
                    {
                        MessageBox.Show("Programador não encontrado", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (tipoTarefaEntity == null)
                    {
                        MessageBox.Show("Tipo de tarefa não encontrado", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 4. Atualizar os campos da tarefa
                    tarefa.Descricao = descricao;
                    tarefa.IdProgramador = programadorEntity.Id;
                    tarefa.Programador = programadorEntity;
                    tarefa.IdTipoTarefa = tipoTarefaEntity.Id;
                    tarefa.TipoTarefa = tipoTarefaEntity;
                    tarefa.Ordem = txtOrdem.Text;
                    tarefa.StoryPoints = !string.IsNullOrWhiteSpace(txtStoryPoints.Text) ?
                                        int.Parse(txtStoryPoints.Text) : 1;
                    tarefa.DataPrevistaInicio = dtInicio.Value;
                    tarefa.DataPrevistaFim = dtFim.Value;
                    tarefa.EstadoAtual = txtEstado.Text;

                    // 5. Marcar como modificado e salvar
                    db.Entry(tarefa).State = EntityState.Modified;
                    db.SaveChanges();

                    MessageBox.Show("Tarefa atualizada com sucesso!", "Sucesso",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (DbUpdateException dbEx)
                {
                    var errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                    MessageBox.Show($"Erro ao atualizar no banco de dados: {errorMessage}", "Erro",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
