using System;
using System.Linq;
using System.Windows.Forms;

namespace iTasks
{
    public partial class frmGereUtilizadores : Form
    {

        public frmGereUtilizadores(string currentUsername)
        {
            InitializeComponent();
            lstListaGestores.Items.Clear();
            LerBaseDados();
            
        }

        private void LerBaseDados()
        {
            LerGestores();
            LerProgramadores();
            // Load departments combo
            using (var db = new iTask())
            {
                cbDepartamento.DataSource = db.Utilizadores.OfType<Gestor>()
                                             .Select(g => g.Departamento)
                                             .Distinct()
                                             .ToList();

                // Load managers combo for programmers
                cbGestorProg.DataSource = db.Utilizadores.OfType<Gestor>()
                                             .Select(g => g.Username)
                                             .ToList();
            }
        }

        private void LerGestores()
        {
            lstListaGestores.Items.Clear();
            using (var db = new iTask())
            {
                var gestores = db.Utilizadores.OfType<Gestor>().ToList();
                foreach (var gestor in gestores)
                {
                    lstListaGestores.Items.Add($"{gestor.Username} -> {gestor.Departamento}");
                }
            }
        }

        private void LerProgramadores()
        {
            lstListaProgramadores.Items.Clear();
            using (var db = new iTask())
            {
                var programadores = db.Utilizadores.OfType<Programador>().ToList();
                foreach (var programador in programadores)
                {
                    lstListaProgramadores.Items.Add($"{programador.Username} -> {programador.NivelExperiencia}");
                }
            }
        }
        private void btGravarGestor_Click(object sender, EventArgs e)
        {

            string nome = txtNomeGestor.Text;
            string username = txtUsernameGestor.Text;
            string password = txtPasswordGestor.Text;
            string departamento = cbDepartamento.Text;

            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(departamento))
            {
                MessageBox.Show("Por favor, preencha todos os campos obrigatórios.", "Aviso",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string gereUtilizadores = chkGereUtilizadores.Checked ? "Sim" : "Não";

            using (var db = new iTask())
            {
                if (!db.Utilizadores.Any(u => u.Username == username))
                {
                    var gestor = new Gestor // Explicitly create as Gestor
                    {
                        Nome = nome,
                        Username = username,
                        Password = password,
                        TipoUtilizador = "Gestor",
                        Departamento = departamento,
                        GereUtilizadores = gereUtilizadores
                    };

                    db.Utilizadores.Add(gestor);
                    db.SaveChanges();
                    LerBaseDados();
                }
                else
                {
                    MessageBox.Show("Este username já está em uso. Por favor escolha outro.", "Erro",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btGravarProg_Click(object sender, EventArgs e)
        {
            string nome = txtNomeProg.Text;
            string username = txtUsernameProg.Text;
            string password = txtPasswordProg.Text;
            string nivel = cbNivelProg.Text;
            string gestor = cbGestorProg.Text;

            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(nivel) ||
                string.IsNullOrWhiteSpace(gestor))
            {
                MessageBox.Show("Por favor, preencha todos os campos obrigatórios.", "Aviso",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            using (var db = new iTask())
            {
                var selectedGestor = db.Utilizadores.OfType<Gestor>()
                                       .FirstOrDefault(g => g.Username == cbGestorProg.Text);

                var programador = new Programador
                {
                    Nome = txtNomeProg.Text,
                    Username = txtUsernameProg.Text,
                    Password = txtPasswordProg.Text, // Note: Should be hashed in production
                    TipoUtilizador = "Programador",
                    NivelExperiencia = cbNivelProg.Text,
                    Gestor = selectedGestor
                };

                db.Utilizadores.Add(programador);
                db.SaveChanges();
                LerBaseDados();
                MessageBox.Show("Programador criado com sucesso!", "Sucesso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}