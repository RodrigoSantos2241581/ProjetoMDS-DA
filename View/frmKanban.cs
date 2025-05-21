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
                }
                else
                {
                    // Enable the menu item if the user has permission
                    gerirUtilizadoresToolStripMenuItem.Enabled = true;
                    gerirTiposDeTarefasToolStripMenuItem.Enabled = true;

                }
            }
        }


    }
}
