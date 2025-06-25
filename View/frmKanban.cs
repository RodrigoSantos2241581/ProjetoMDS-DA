using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace iTasks
{
    public partial class frmKanban : Form
    {
        string Utilizador;
        private bool GerirUtilizadores;
        private bool IsGestor;
        private int ProgramadorId; // Only set if the user is a programmer

        public frmKanban(string username)
        {
            InitializeComponent();
            Utilizador = username; // Store the username
            label1.Text = "Bem vindo: <" + Utilizador + ">"; // Welcome message
            CheckPermissions();
            LoadTasks();
            DesselecionarLists();
        }

        private void LoadTasks()
        {
            lstTodo.DataSource = null;
            lstDoing.DataSource = null;
            lstDone.DataSource = null;

            using (var db = new iTask())
            {
                IQueryable<Tarefa> tarefasQuery = db.Tarefas;

                // Obter o gestor atual
                var gestorAtual = db.Utilizadores.OfType<Gestor>()
                                      .FirstOrDefault(g => g.Username == Utilizador);

                if (gestorAtual != null)
                {
                    // 1. Obter IDs dos programadores deste gestor
                    var idsProgramadores = db.Utilizadores.OfType<Programador>()
                                            .Where(p => p.Gestor.Id == gestorAtual.Id)
                                            .Select(p => p.Id)
                                            .ToList();

                    // 2. Filtrar tarefas:
                    // - OU são dos programadores deste gestor
                    // - OU não têm programador atribuído (para o gestor poder atribuir)
                    tarefasQuery = tarefasQuery.Where(t =>
                        t.IdProgramador == null ||
                        idsProgramadores.Contains(t.IdProgramador));
                }
                else
                {
                    // Se for programador, mostrar apenas suas tarefas
                    var programadorAtual = db.Utilizadores.OfType<Programador>()
                                            .FirstOrDefault(p => p.Username == Utilizador);
                    if (programadorAtual != null)
                    {
                        tarefasQuery = tarefasQuery.Where(t => t.IdProgramador == programadorAtual.Id);
                    }
                }

                var tarefas = tarefasQuery.ToList();

                lstTodo.DataSource = tarefas
                    .Where(t => t.EstadoAtual == "To Do")
                    .Select(t => $"{t.Descricao} => {t.Programador?.Nome ?? "Ninguém"}")
                    .ToList();

                lstDoing.DataSource = tarefas
                    .Where(t => t.EstadoAtual == "Doing")
                    .Select(t => $"{t.Descricao} => {t.Programador?.Nome ?? "Ninguém"}")
                    .ToList();

                lstDone.DataSource = tarefas
                    .Where(t => t.EstadoAtual == "Done")
                    .Select(t => $"{t.Descricao} => {t.Programador?.Nome ?? "Ninguém"}")
                    .ToList();
            }
        }

        private void DesselecionarLists()
        {
            lstTodo.ClearSelected();
            lstDoing.ClearSelected();
            lstDone.ClearSelected();
        }

        private void CheckPermissions()
        {
            using (var db = new iTask())
            {
                // Check if user is a manager
                var gestor = db.Utilizadores.OfType<Gestor>().FirstOrDefault(u => u.Username == Utilizador);
                if (gestor != null)
                {
                    GerirUtilizadores = gestor.GereUtilizadores == "Sim";
                    IsGestor = true;
                }
                else
                {
                    // Check if user is a programmer
                    var programador = db.Utilizadores.OfType<Programador>().FirstOrDefault(u => u.Username == Utilizador);
                    if (programador != null)
                    {
                        ProgramadorId = programador.Id;
                        IsGestor = false;
                    }
                }

                // Set UI elements based on permissions
                gerirUtilizadoresToolStripMenuItem.Enabled = IsGestor && GerirUtilizadores;
                gerirTiposDeTarefasToolStripMenuItem.Enabled = IsGestor && GerirUtilizadores;
                btNova.Enabled = IsGestor && GerirUtilizadores;
            }
        }

        private bool IsMyTask(string taskDescription, string estado)
        {
            if (IsGestor) return true; // Managers can move all tasks

            using (var db = new iTask())
            {
                var tarefa = db.Tarefas.FirstOrDefault(t =>
                    t.Descricao == taskDescription &&
                    t.EstadoAtual == estado);

                return tarefa != null && tarefa.IdProgramador == ProgramadorId;
            }
        }

        private void gerirUtilizadoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmGereUtilizadores gereUtilizadoresForm = new frmGereUtilizadores(Utilizador);
            gereUtilizadoresForm.Show();
        }

        private void gerirTiposDeTarefasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmGereTiposTarefas gereTiposTarefasForm = new frmGereTiposTarefas();
            gereTiposTarefasForm.Show();
        }

        private void btNova_Click(object sender, EventArgs e)
        {
            int? tarefaId = null;

            // Verifica se há exatamente UMA tarefa selecionada
            if (lstTodo.SelectedItems.Count == 1)
            {
                string selectedTask = lstTodo.SelectedItem.ToString();
                string descricaoTarefa = selectedTask.Split(new[] { " => " }, StringSplitOptions.None)[0];
                tarefaId = ObterIdTarefa(descricaoTarefa, "To Do");
            }
            else if (lstDoing.SelectedItems.Count == 1)
            {
                string selectedTask = lstDoing.SelectedItem.ToString();
                string descricaoTarefa = selectedTask.Split(new[] { " => " }, StringSplitOptions.None)[0];
                tarefaId = ObterIdTarefa(descricaoTarefa, "Doing");
            }
            else if (lstDone.SelectedItems.Count == 1)
            {
                string selectedTask = lstDone.SelectedItem.ToString();
                string descricaoTarefa = selectedTask.Split(new[] { " => " }, StringSplitOptions.None)[0];
                tarefaId = ObterIdTarefa(descricaoTarefa, "Done");
            }

            int gestorId = ObterIdGestor();

            if (tarefaId.HasValue)
            {
                // Modo edição - mostra detalhes da tarefa existente
                frmDetalhesTarefa detalhesTarefaForm = new frmDetalhesTarefa(gestorId, tarefaId);
                detalhesTarefaForm.Show();
            }
            else if (lstTodo.SelectedItems.Count == 0 &&
                     lstDoing.SelectedItems.Count == 0 &&
                     lstDone.SelectedItems.Count == 0)
            {
                // Modo criação - nova tarefa
                frmDetalhesTarefa detalhesTarefaForm = new frmDetalhesTarefa(gestorId);
                detalhesTarefaForm.Show();
            }
            else
            {
                MessageBox.Show("Selecione apenas UMA tarefa para editar ou nenhuma para criar nova.",
                              "Ação Inválida",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
            }

            DesselecionarLists();
        }

        private int? ObterIdTarefa(string descricao, string estado)
        {
            using (var db = new iTask())
            {
                var tarefa = db.Tarefas.FirstOrDefault(t => t.Descricao == descricao && t.EstadoAtual == estado);
                return tarefa?.Id;
            }
        }

        private int ObterIdGestor()
        {
            using (var db = new iTask())
            {
                var gestor = db.Utilizadores.OfType<Gestor>().FirstOrDefault(u => u.Username == Utilizador);
                return gestor?.Id ?? 0; // Retorna 0 se não encontrar o gestor
            }
        }

        private void btSetDoing_Click(object sender, EventArgs e)
        {
            if (lstTodo.SelectedItem != null)
            {
                string selectedTask = lstTodo.SelectedItem.ToString();
                string descricaoTarefa = selectedTask.Split(new[] { " => " }, StringSplitOptions.None)[0];

                if (!IsMyTask(descricaoTarefa, "To Do") && !IsGestor)
                {
                    MessageBox.Show("Só pode mover as suas próprias tarefas.", "Permissão Negada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var db = new iTask())
                {
                    var tarefa = db.Tarefas.FirstOrDefault(t => t.Descricao == descricaoTarefa && t.EstadoAtual == "To Do");
                    if (tarefa != null)
                    {
                        // Verificar se o programador já tem 2 tarefas em Doing
                        if (tarefa.IdProgramador != 0 && !ProgramadorPodeAssumirMaisTarefas(tarefa.IdProgramador) && !IsGestor)
                        {
                            MessageBox.Show("Ja tens 2 tarefas em progresso.", "Limite Atingido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        tarefa.DataRealInicio = DateTime.Now;
                        tarefa.EstadoAtual = "Doing";
                        db.SaveChanges();
                        LoadTasks();
                    }
                }
            }
            DesselecionarLists();
        }

        private void btSetTodo_Click(object sender, EventArgs e)
        {
            if (lstDoing.SelectedItem != null)
            {
                string selectedTask = lstDoing.SelectedItem.ToString();
                string descricaoTarefa = selectedTask.Split(new[] { " => " }, StringSplitOptions.None)[0];

                if (!IsMyTask(descricaoTarefa, "Doing") && !IsGestor)
                {
                    MessageBox.Show("Só pode mover as suas próprias tarefas.", "Permissão Negada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var db = new iTask())
                {
                    var tarefa = db.Tarefas.FirstOrDefault(t => t.Descricao == descricaoTarefa && t.EstadoAtual == "Doing");
                    if (tarefa != null)
                    {
                        tarefa.EstadoAtual = "To Do";
                        db.SaveChanges();
                        LoadTasks();
                    }
                }
            }
            DesselecionarLists();
        }

        private void btSetDone_Click(object sender, EventArgs e)
        {
            if (lstDoing.SelectedItem != null)
            {
                string selectedTask = lstDoing.SelectedItem.ToString();
                string descricaoTarefa = selectedTask.Split(new[] { " => " }, StringSplitOptions.None)[0];

                if (!IsMyTask(descricaoTarefa, "Doing") && !IsGestor)
                {
                    MessageBox.Show("Só pode mover as suas próprias tarefas.", "Permissão Negada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var db = new iTask())
                {
                    var tarefa = db.Tarefas.FirstOrDefault(t => t.Descricao == descricaoTarefa && t.EstadoAtual == "Doing");
                    if (tarefa != null)
                    {
                        tarefa.DataRealFim = DateTime.Now;
                        tarefa.EstadoAtual = "Done";
                        db.SaveChanges();
                        LoadTasks();
                    }
                }
            }
            DesselecionarLists();
        }

        private void tarefasTerminadasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmConsultarTarefasConcluidas consultarTarefasConcluidasForm = new frmConsultarTarefasConcluidas(Utilizador);
            consultarTarefasConcluidasForm.Show();
        }

        private void tarefasEmCursoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmConsultaTarefasEmCurso consultaTarefasEmCursoForm = new frmConsultaTarefasEmCurso(Utilizador);
            consultaTarefasEmCursoForm.Show();
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show(
                "Deseja realmente sair?",
                "Confirmar Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (resultado == DialogResult.Yes)
            {
                MessageBox.Show("Logout com sucesso.", "Logout", MessageBoxButtons.OK, MessageBoxIcon.Information);
                frmLogin loginForm = new frmLogin();
                loginForm.Show();

                this.Close();
            }
        }

        private bool ProgramadorPodeAssumirMaisTarefas(int programadorId)
        {
            using (var db = new iTask())
            {
                int tarefasDoing = db.Tarefas.Count(t => t.IdProgramador == programadorId && t.EstadoAtual == "Doing");
                return tarefasDoing < 2;
            }
        }

        private void exportarParaCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new iTask())
                {
                    // Get the current manager
                    var gestorAtual = db.Utilizadores.OfType<Gestor>()
                                          .FirstOrDefault(g => g.Username == Utilizador);

                    if (gestorAtual == null)
                    {
                        MessageBox.Show("Apenas gestores podem exportar tarefas.", "Permissão Negada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Get all tasks for this manager's programmers
                    var tarefas = db.Tarefas
                        .Where(t => t.Programador != null && t.Programador.Gestor.Id == gestorAtual.Id)
                        .OrderBy(t => t.Programador.Nome)
                        .ThenBy(t => t.EstadoAtual)
                        .ToList();

                    if (tarefas.Count == 0)
                    {
                        MessageBox.Show("Não existem tarefas para exportar.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Create save file dialog
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "CSV files (*.csv)|*.csv",
                        Title = "Exportar Tarefas para CSV",
                        FileName = $"Tarefas_Gestor_{gestorAtual.Nome}_{DateTime.Now:yyyyMMdd}.csv"
                    };

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8))
                        {
                            // Write header
                            writer.WriteLine("Programador;Descrição;Estado;Data Prevista Início;Data Prevista Fim;Tipo Tarefa;Data Real Início;Data Real Fim");

                            // Write data
                            foreach (var tarefa in tarefas)
                            {
                                writer.WriteLine(
                                    $"{EscapeCsv(tarefa.Programador?.Nome)};" +
                                    $"{EscapeCsv(tarefa.Descricao)};" +
                                    $"{EscapeCsv(tarefa.EstadoAtual)};" +
                                    $"{tarefa.DataPrevistaInicio:yyyy-MM-dd};" +
                                    $"{tarefa.DataPrevistaFim:yyyy-MM-dd};" +
                                    $"{EscapeCsv(tarefa.TipoTarefa.Descricao)};" +
                                    $"{(tarefa.DataRealInicio.HasValue ? tarefa.DataRealInicio.Value.ToString("yyyy-MM-dd") : "")};" +
                                    $"{(tarefa.DataRealFim.HasValue ? tarefa.DataRealFim.Value.ToString("yyyy-MM-dd") : "")}"
                                );
                            }
                        }

                        MessageBox.Show($"Tarefas exportadas com sucesso para {saveFileDialog.FileName}", "Exportação Concluída", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro ao exportar para CSV: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            // Escape quotes and wrap in quotes if contains semicolon
            if (value.Contains(";") || value.Contains("\""))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }
            return value;
        }
    }
}