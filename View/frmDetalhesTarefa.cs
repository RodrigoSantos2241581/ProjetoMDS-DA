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

    public partial class frmDetalhesTarefa : Form
    {
        public frmDetalhesTarefa(int id)
        {
            InitializeComponent();
            CarregarProgramadores(id);
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


        private void btGravar_Click(object sender, EventArgs e)
        {

        }
    }
}
