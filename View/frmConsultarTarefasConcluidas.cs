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
    public partial class frmConsultarTarefasConcluidas : Form
    {
        public frmConsultarTarefasConcluidas(string currentUser)
        {
            InitializeComponent();
            VerTarefasConcluidas(currentUser);
        }

        public void VerTarefasConcluidas(string user)
        {
            //mostra as tarefas no estado done
            using (var db = new iTask())
            {
                var tarefasConcluidas = db.Tarefas
                    .Where(t => t.EstadoAtual == "Done" && t.Programador.Username == user)
                    .Select(t => new
                    {
                        t.Id,
                        t.Descricao,
                        t.DataCriacao,
                        t.DataRealFim,
                        t.Programador.Username
                    })
                    .ToList();
                gvTarefasConcluidas.DataSource = tarefasConcluidas;
                // Display the description in the list
                gvTarefasConcluidas.Columns["Id"].Visible = false; // Hide the Id column
                gvTarefasConcluidas.Columns["Descricao"].HeaderText = "Descrição da Tarefa";
                gvTarefasConcluidas.Columns["DataCriacao"].HeaderText = "Data de Criação";
                gvTarefasConcluidas.Columns["DataRealFim"].HeaderText = "Data de Conclusão";
                gvTarefasConcluidas.Columns["Username"].HeaderText = "Programador";
                gvTarefasConcluidas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

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
