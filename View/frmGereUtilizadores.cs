using System;
using System.Data.Entity;
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
                             .Where(g => g.GereUtilizadores == "Sim")
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
                // Verifica se estamos editando um gestor existente
                if (!string.IsNullOrWhiteSpace(txtIdGestor.Text) && int.TryParse(txtIdGestor.Text, out int id))
                {
                    // Modo edição - atualiza o gestor existente
                    var gestorExistente = db.Utilizadores.OfType<Gestor>().FirstOrDefault(g => g.Id == id);

                    if (gestorExistente != null)
                    {
                        // Verifica se o username foi alterado e se já existe
                        if (gestorExistente.Username != username &&
                            db.Utilizadores.Any(u => u.Username == username && u.Id != id))
                        {
                            MessageBox.Show("Este username já está em uso. Por favor escolha outro.", "Erro",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        gestorExistente.Nome = nome;
                        gestorExistente.Username = username;
                        gestorExistente.Password = password;
                        gestorExistente.Departamento = departamento;
                        gestorExistente.GereUtilizadores = gereUtilizadores;

                        db.SaveChanges();
                        LerBaseDados();
                        MessageBox.Show("Gestor atualizado com sucesso!", "Sucesso",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Limpa os campos após a edição
                        txtIdGestor.Clear();
                        return;
                    }
                }
                else
                {
                    // Modo criação - adiciona novo gestor
                    if (!db.Utilizadores.Any(u => u.Username == username))
                    {
                        var gestor = new Gestor
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
                        MessageBox.Show("Gestor criado com sucesso!", "Sucesso",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Este username já está em uso. Por favor escolha outro.", "Erro",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btGravarProg_Click(object sender, EventArgs e)
        {
            string nome = txtNomeProg.Text;
            string username = txtUsernameProg.Text;
            string password = txtPasswordProg.Text;
            string nivel = cbNivelProg.Text;
            string gestorUsername = cbGestorProg.Text;

            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(nivel) ||
                string.IsNullOrWhiteSpace(gestorUsername))
            {
                MessageBox.Show("Por favor, preencha todos os campos obrigatórios.", "Aviso",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var db = new iTask())
            {
                // Filtra apenas gestores que podem gerir utilizadores
                var selectedGestor = db.Utilizadores.OfType<Gestor>()
                                       .FirstOrDefault(g => g.Username == gestorUsername &&
                                                           g.GereUtilizadores == "Sim");

                if (selectedGestor == null)
                {
                    MessageBox.Show("Por favor, selecione um gestor com permissão para gerir utilizadores",
                                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Verifica se estamos editando um programador existente
                if (!string.IsNullOrWhiteSpace(txtIdProg.Text) && int.TryParse(txtIdProg.Text, out int id))
                {
                    // Modo edição - atualiza o programador existente
                    var programadorExistente = db.Utilizadores.OfType<Programador>()
                                                 .FirstOrDefault(p => p.Id == id);

                    if (programadorExistente != null)
                    {
                        // Verifica se o username foi alterado e se já existe
                        if (programadorExistente.Username != username &&
                            db.Utilizadores.Any(u => u.Username == username && u.Id != id))
                        {
                            MessageBox.Show("Este username já está em uso. Por favor escolha outro.", "Erro",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        programadorExistente.Nome = nome;
                        programadorExistente.Username = username;
                        programadorExistente.Password = password;
                        programadorExistente.NivelExperiencia = nivel;
                        programadorExistente.Gestor = selectedGestor;

                        db.SaveChanges();
                        LerBaseDados();
                        MessageBox.Show("Programador atualizado com sucesso!", "Sucesso",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Limpa os campos após a edição
                        txtIdProg.Clear();
                        return;
                    }
                }
                else
                {
                    // Modo criação - adiciona novo programador
                    if (!db.Utilizadores.Any(u => u.Username == username))
                    {
                        var programador = new Programador
                        {
                            Nome = nome,
                            Username = username,
                            Password = password,
                            TipoUtilizador = "Programador",
                            NivelExperiencia = nivel,
                            Gestor = selectedGestor
                        };

                        db.Utilizadores.Add(programador);
                        db.SaveChanges();
                        LerBaseDados();
                        MessageBox.Show("Programador criado com sucesso!", "Sucesso",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Este username já está em uso. Por favor escolha outro.", "Erro",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
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

        private void lstListaGestores_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Verifica se um item está selecionado
            if (lstListaGestores.SelectedItem != null)
            {
                // Obtém o gestor selecionado
                string selectedItem = lstListaGestores.SelectedItem.ToString();
                string username = selectedItem.Split(new[] { " -> " }, StringSplitOptions.None)[0];
                using (var db = new iTask())
                {
                    // Busca o gestor pelo username
                    var gestor = db.Utilizadores.OfType<Gestor>().FirstOrDefault(g => g.Username == username);
                    if (gestor != null)
                    {
                        txtIdGestor.Text = gestor.Id.ToString();
                        txtNomeGestor.Text = gestor.Nome;
                        txtUsernameGestor.Text = gestor.Username;
                        txtPasswordGestor.Text = gestor.Password;
                        cbDepartamento.Text = gestor.Departamento;
                        chkGereUtilizadores.Checked = gestor.GereUtilizadores == "Sim";
                    }
                }
            }
        }

        private void lstListaProgramadores_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstListaProgramadores.SelectedItem != null)
            {
                string selectedItem = lstListaProgramadores.SelectedItem.ToString();
                string username = selectedItem.Split(new[] { " -> " }, StringSplitOptions.None)[0];

                using (var db = new iTask())
                {
                    var programador = db.Utilizadores.OfType<Programador>()
                                        .Include(p => p.Gestor) // Carrega o gestor relacionado
                                        .FirstOrDefault(p => p.Username == username);

                    if (programador != null)
                    {
                        txtIdProg.Text = programador.Id.ToString();
                        txtNomeProg.Text = programador.Nome;
                        txtUsernameProg.Text = programador.Username;
                        txtPasswordProg.Text = programador.Password;
                        cbNivelProg.Text = programador.NivelExperiencia;

                        // Verifica se o gestor existe e atualiza o combobox
                        if (programador.Gestor != null)
                        {
                            cbGestorProg.SelectedItem = programador.Gestor.Username;
                        }
                        else
                        {
                            cbGestorProg.SelectedIndex = -1;
                        }
                    }
                }
            }
        }
    }
}