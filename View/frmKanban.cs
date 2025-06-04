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
    public partial class frmKanban : Form
    {
        string Utillizador;
        private bool GerirUtilizadores;
        public frmKanban(string username)
        {
            InitializeComponent();
            Utillizador = username; // Store the username
            label1.Text = "Bem vindo: <" + Utillizador + ">"; // Welcome message
            CheckPermissions();
            lstTodo.DataSource = null;
            lstDoing.DataSource = null;
            lstDone.DataSource = null;
            // Load tasks from the database
            using (var db = new iTask())
            {
                var tarefas = db.Tarefas.ToList();
                lstTodo.DataSource = tarefas.Where(t => t.EstadoAtual == "To Do").Select(t => t.Descricao).ToList();
                lstDoing.DataSource = tarefas.Where(t => t.EstadoAtual == "Doing").Select(t => t.Descricao).ToList();
                lstDone.DataSource = tarefas.Where(t => t.EstadoAtual == "Done").Select(t => t.Descricao).ToList();
            }

            DesselecionarLists();
        }

        private void DesselecionarLists()
        {
            lstTodo.ClearSelected();
            lstDoing.ClearSelected();
            lstDone.ClearSelected();
        }
        private void gerirUtilizadoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmGereUtilizadores gereUtilizadoresForm = new frmGereUtilizadores(Utillizador);
            gereUtilizadoresForm.Show();
        }

        private void gerirTiposDeTarefasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmGereTiposTarefas gereTiposTarefasForm = new frmGereTiposTarefas();
            gereTiposTarefasForm.Show();
        }

        private void CheckPermissions()
        {
            using (var db = new iTask())
            {
                var currentUser = db.Utilizadores.OfType<Gestor>().FirstOrDefault(u => u.Username == Utillizador);

                GerirUtilizadores = currentUser?.GereUtilizadores == "Sim";

                if (!GerirUtilizadores)
                {
                    // Disable the menu item if the user does not have permission
                    gerirUtilizadoresToolStripMenuItem.Enabled = false;
                    gerirTiposDeTarefasToolStripMenuItem.Enabled = false;
                    btNova.Enabled = false;
                }
                else
                {
                    // Enable the menu item if the user has permission
                    gerirUtilizadoresToolStripMenuItem.Enabled = true;
                    gerirTiposDeTarefasToolStripMenuItem.Enabled = true;
                    btNova.Enabled = true;
                }
            }
        }

        private void btNova_Click(object sender, EventArgs e)
        {
            //verifica se alguma tarefa esta selecionada
            if (lstTodo.SelectedItem != null || lstDoing.SelectedItem != null || lstDone.SelectedItem != null)
            {
                VerDetalhes();
            }
            else {
                int gestorId = 0;
                using (var db = new iTask())
                {
                    var gestor = db.Utilizadores.OfType<Gestor>().FirstOrDefault(u => u.Username == Utillizador);
                    if (gestor != null)
                    {
                        gestorId = gestor.Id;
                    }
                }

                frmDetalhesTarefa detalhesTarefaForm = new frmDetalhesTarefa(gestorId);
                detalhesTarefaForm.Show();
            }

            DesselecionarLists();

        }

        private void btSetDoing_Click(object sender, EventArgs e)
        {
            // Get what is selected in the To Do list
            if (lstTodo.SelectedItem != null)
            {
                string selectedTask = lstTodo.SelectedItem.ToString();
                using (var db = new iTask())
                {
                    var tarefa = db.Tarefas.FirstOrDefault(t => t.Descricao == selectedTask && t.EstadoAtual == "To Do");
                    if (tarefa != null)
                    {
                        tarefa.DataRealInicio = DateTime.Now; // Set the start date
                        tarefa.EstadoAtual = "Doing";
                        db.SaveChanges();
                        // Refresh the lists
                        lstTodo.DataSource = db.Tarefas.Where(t => t.EstadoAtual == "To Do").Select(t => t.Descricao).ToList();
                        lstDoing.DataSource = db.Tarefas.Where(t => t.EstadoAtual == "Doing").Select(t => t.Descricao).ToList();
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
                using (var db = new iTask())
                {
                    var tarefa = db.Tarefas.FirstOrDefault(t => t.Descricao == selectedTask && t.EstadoAtual == "Doing");
                    if (tarefa != null)
                    {
                        tarefa.EstadoAtual = "To Do";
                        db.SaveChanges();
                        // Refresh the lists
                        lstTodo.DataSource = db.Tarefas.Where(t => t.EstadoAtual == "To Do").Select(t => t.Descricao).ToList();
                        lstDoing.DataSource = db.Tarefas.Where(t => t.EstadoAtual == "Doing").Select(t => t.Descricao).ToList();
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
                using (var db = new iTask())
                {
                    var tarefa = db.Tarefas.FirstOrDefault(t => t.Descricao == selectedTask && t.EstadoAtual == "Doing");
                    if (tarefa != null)
                    {
                        tarefa.DataRealFim = DateTime.Now;
                        tarefa.EstadoAtual = "Done";
                        db.SaveChanges();
                        lstDoing.DataSource = db.Tarefas.Where(t => t.EstadoAtual == "Doing").Select(t => t.Descricao).ToList();
                        lstDone.DataSource = db.Tarefas.Where(t => t.EstadoAtual == "Done").Select(t => t.Descricao).ToList();
                    }
                }
            }
            DesselecionarLists();
        }

        private void tarefasTerminadasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmConsultarTarefasConcluidas consultarTarefasConcluidasForm = new frmConsultarTarefasConcluidas(Utillizador);
            consultarTarefasConcluidasForm.Show();
        }

        private void tarefasEmCursoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmConsultaTarefasEmCurso consultaTarefasEmCursoForm = new frmConsultaTarefasEmCurso(Utillizador);
            consultaTarefasEmCursoForm.Show();
        }

        private void VerDetalhes()
        {
            if (lstTodo.SelectedItem != null)
            {
                string selectedTask = lstTodo.SelectedItem.ToString();
                using (var db = new iTask())
                {
                    var tarefa = db.Tarefas.FirstOrDefault(t => t.Descricao == selectedTask && t.EstadoAtual == "To Do");
                    if (tarefa != null)
                    {
                        frmDetalhesTarefa detalhesTarefaForm = new frmDetalhesTarefa(tarefa.Id);
                        detalhesTarefaForm.Show();
                    }
                }
            }
            else if (lstDoing.SelectedItem != null)
            {
                string selectedTask = lstDoing.SelectedItem.ToString();
                using (var db = new iTask())
                {
                    var tarefa = db.Tarefas.FirstOrDefault(t => t.Descricao == selectedTask && t.EstadoAtual == "Doing");
                    if (tarefa != null)
                    {
                        frmDetalhesTarefa detalhesTarefaForm = new frmDetalhesTarefa(tarefa.Id);
                        detalhesTarefaForm.Show();
                    }
                }
            }
            else if (lstDone.SelectedItem != null)
            {
                string selectedTask = lstDone.SelectedItem.ToString();
                using (var db = new iTask())
                {
                    var tarefa = db.Tarefas.FirstOrDefault(t => t.Descricao == selectedTask && t.EstadoAtual == "Done");
                    if (tarefa != null)
                    {
                        frmDetalhesTarefa detalhesTarefaForm = new frmDetalhesTarefa(tarefa.Id);
                        detalhesTarefaForm.Show();
                    }
                }
            }
            DesselecionarLists();
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
    }
}
