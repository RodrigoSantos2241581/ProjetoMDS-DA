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
        private readonly string _currentUser;
        private readonly bool _isGestor;

        public frmConsultaTarefasEmCurso(string currentUser, bool isGestor = true)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _isGestor = isGestor;
            LoadTasksInProgress();
        }

        public void LoadTasksInProgress()
        {
            using (var db = new iTask())
            {
                var query = db.Tarefas.Where(t => t.EstadoAtual == "To Do" || t.EstadoAtual == "Doing");

                if (_isGestor)
                {
                    var gestorAtual = db.Utilizadores.OfType<Gestor>()
                                          .FirstOrDefault(g => g.Username == _currentUser);

                    if (gestorAtual != null)
                    {
                        var idsProgramadores = db.Utilizadores.OfType<Programador>()
                                                .Where(p => p.Gestor.Id == gestorAtual.Id)
                                                .Select(p => p.Id)
                                                .ToList();

                        query = query.Where(t => t.IdProgramador != null &&
                                                idsProgramadores.Contains(t.IdProgramador));
                    }
                }
                else
                {
                    query = query.Where(t => t.Programador.Username == _currentUser);
                }

                var tarefasEmCurso = query
                    .OrderByDescending(t => t.EstadoAtual)
                    .Select(t => new
                    {
                        t.Id,
                        t.Descricao,
                        t.DataCriacao,
                        t.DataPrevistaInicio,
                        t.DataPrevistaFim,
                        t.DataRealInicio,
                        Programador = t.Programador.Username,
                        TipoTarefa = t.TipoTarefa.Descricao,
                        t.EstadoAtual,
                        t.StoryPoints
                    })
                    .AsEnumerable()  // Executa a query e traz os dados para memória
                    .Select(t => new
                    {
                        t.Id,
                        Descricao = t.Descricao ?? "Sem descrição",
                        DataCriacao = t.DataCriacao,
                        Programador = t.Programador ?? "Não atribuído",
                        TipoTarefa = t.TipoTarefa,
                        Estado = t.EstadoAtual,
                        StoryPoints = t.StoryPoints,
                        // Cálculos feitos em memória
                        DiasPrevistos = (t.DataPrevistaFim - t.DataPrevistaInicio).TotalDays.ToString("0.0"),
                        DiasDecorridos = (t.DataRealInicio.HasValue)
                                       ? (DateTime.Now - t.DataRealInicio.Value).TotalDays.ToString("0.0")
                                       : "N/A",
                        DiasRestantes = (t.DataPrevistaFim - DateTime.Now).TotalDays.ToString("+0.0;-0.0;0.0"),
                        Status = (t.DataPrevistaFim < DateTime.Now && t.EstadoAtual != "Done")
                                ? "Atrasada"
                                : "No prazo"
                    })
                    .ToList();

                gvTarefasEmCurso.DataSource = tarefasEmCurso;

                // Configuração das colunas
                gvTarefasEmCurso.Columns["Id"].Visible = false;
                gvTarefasEmCurso.Columns["Descricao"].HeaderText = "Descrição";
                gvTarefasEmCurso.Columns["DataCriacao"].HeaderText = "Criação";
                gvTarefasEmCurso.Columns["Programador"].HeaderText = "Programador";
                gvTarefasEmCurso.Columns["TipoTarefa"].HeaderText = "Tipo";
                gvTarefasEmCurso.Columns["Estado"].HeaderText = "Estado";
                gvTarefasEmCurso.Columns["StoryPoints"].HeaderText = "SP";
                gvTarefasEmCurso.Columns["DiasPrevistos"].HeaderText = "Dias Previstos";
                gvTarefasEmCurso.Columns["DiasDecorridos"].HeaderText = "Dias Decorridos";
                gvTarefasEmCurso.Columns["DiasRestantes"].HeaderText = "Dias Restantes";
                gvTarefasEmCurso.Columns["Status"].HeaderText = "Status";

                // Formatação das datas
                gvTarefasEmCurso.Columns["DataCriacao"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

                // Formatação das colunas numéricas
                gvTarefasEmCurso.Columns["StoryPoints"].DefaultCellStyle.Alignment =
                gvTarefasEmCurso.Columns["DiasPrevistos"].DefaultCellStyle.Alignment =
                gvTarefasEmCurso.Columns["DiasDecorridos"].DefaultCellStyle.Alignment =
                gvTarefasEmCurso.Columns["DiasRestantes"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;

                // Destaque para status
                gvTarefasEmCurso.CellFormatting += (sender, e) =>
                {
                    if (e.ColumnIndex == gvTarefasEmCurso.Columns["Status"].Index && e.Value != null)
                    {
                        if (e.Value.ToString() == "Atrasada")
                        {
                            e.CellStyle.ForeColor = Color.Red;
                            e.CellStyle.Font = new Font(gvTarefasEmCurso.Font, FontStyle.Bold);
                        }
                        else
                        {
                            e.CellStyle.ForeColor = Color.Green;
                        }
                    }

                    if (e.ColumnIndex == gvTarefasEmCurso.Columns["DiasRestantes"].Index && e.Value != null)
                    {
                        if (e.Value.ToString().StartsWith("-"))
                        {
                            e.CellStyle.ForeColor = Color.Red; // Atrasada
                        }
                        else
                        {
                            e.CellStyle.ForeColor = Color.Green; // No prazo
                        }
                    }
                };

                gvTarefasEmCurso.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void btFechar_Click(object sender, EventArgs e)
        {
            this.Close();

            var kanbanForm = Application.OpenForms.OfType<frmKanban>().FirstOrDefault();
            kanbanForm?.Show();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Funcionalidade de exportação será implementada em breve.", "Exportar",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAtualizar_Click(object sender, EventArgs e)
        {
            LoadTasksInProgress();
        }
    }
}