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
    public partial class frmConsultaTarefasEmCurso : Form
    {
        public frmConsultaTarefasEmCurso(string currentUser)
        {
            InitializeComponent();
            VerTarefasEmCurso(currentUser);
        }

        private void VerTarefasEmCurso(string user)
        {
            //mostra as tarefas no estado em curso
            using (var db = new iTask())
            {
                var tarefasEmCurso = db.Tarefas
                    .Where(t => (t.EstadoAtual == "To Do" || t.EstadoAtual == "Doing")&& t.Programador.Username == user)
                    .Select(t => new
                    {
                        t.Id,
                        t.Descricao,
                        t.DataCriacao,
                        t.DataPrevistaFim,
                        t.Programador.Username
                    })
                    .ToList();
                gvTarefasEmCurso.DataSource = tarefasEmCurso;
                // Display the description in the list
                gvTarefasEmCurso.Columns["Id"].Visible = false; // Hide the Id column
                gvTarefasEmCurso.Columns["Descricao"].HeaderText = "Descrição da Tarefa";
                gvTarefasEmCurso.Columns["DataCriacao"].HeaderText = "Data de Criação";
                gvTarefasEmCurso.Columns["DataPrevistaFim"].HeaderText = "Data Prevista de Fim";
                gvTarefasEmCurso.Columns["Username"].HeaderText = "Programador";
                gvTarefasEmCurso.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
