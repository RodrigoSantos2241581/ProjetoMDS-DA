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
        private readonly string _currentUser;
        private readonly bool _isGestor;

        public frmConsultarTarefasConcluidas(string currentUser, bool isGestor = true)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _isGestor = isGestor;
            LoadConcludedTasks();
        }

        public void LoadConcludedTasks()
        {
            using (var db = new iTask())
            {
                var query = db.Tarefas.Where(t => t.EstadoAtual == "Done");

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

                var tarefasConcluidas = query
                    .OrderByDescending(t => t.DataRealFim)
                    .Select(t => new
                    {
                        t.Id,
                        t.Descricao,
                        t.DataCriacao,
                        t.DataRealFim,
                        Programador = t.Programador.Username,
                        TipoTarefa = t.TipoTarefa.Descricao,
                        t.DataPrevistaInicio,
                        t.DataPrevistaFim,
                        t.DataRealInicio
                    })
                    .AsEnumerable()  // Executa a query e traz os dados para memória
                    .Select(t => new
                    {
                        t.Id,
                        Descricao = t.Descricao ?? "Sem descrição",
                        DataCriacao = t.DataCriacao,
                        DataConclusao = t.DataRealFim,
                        Programador = t.Programador ?? "Não atribuído",
                        TipoTarefa = t.TipoTarefa,
                        // Cálculos feitos em memória
                        DiasPrevistos = (t.DataPrevistaFim - t.DataPrevistaInicio).TotalDays.ToString("0.0"),
                        DiasReais = (t.DataRealFim.HasValue && t.DataRealInicio.HasValue)
                                   ? (t.DataRealFim.Value - t.DataRealInicio.Value).TotalDays.ToString("0.0")
                                   : "N/A",
                        DiferencaDias = (t.DataRealFim.HasValue && t.DataRealInicio.HasValue)
                                       ? ((t.DataRealFim.Value - t.DataRealInicio.Value).TotalDays -
                                          (t.DataPrevistaFim - t.DataPrevistaInicio).TotalDays).ToString("+0.0;-0.0;0.0")
                                       : "N/A"
                    })
                    .ToList();

                gvTarefasConcluidas.DataSource = tarefasConcluidas;

                // Configuração das colunas
                gvTarefasConcluidas.Columns["Id"].Visible = false;
                gvTarefasConcluidas.Columns["Descricao"].HeaderText = "Descrição";
                gvTarefasConcluidas.Columns["DataCriacao"].HeaderText = "Criação";
                gvTarefasConcluidas.Columns["DataConclusao"].HeaderText = "Conclusão";
                gvTarefasConcluidas.Columns["Programador"].HeaderText = "Programador";
                gvTarefasConcluidas.Columns["TipoTarefa"].HeaderText = "Tipo";
                gvTarefasConcluidas.Columns["DiasPrevistos"].HeaderText = "Dias Previstos";
                gvTarefasConcluidas.Columns["DiasReais"].HeaderText = "Dias Reais";
                gvTarefasConcluidas.Columns["DiferencaDias"].HeaderText = "Diferença";

                // Formatação das datas
                gvTarefasConcluidas.Columns["DataCriacao"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                gvTarefasConcluidas.Columns["DataConclusao"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

                // Formatação das colunas numéricas
                gvTarefasConcluidas.Columns["DiasPrevistos"].DefaultCellStyle.Alignment =
                gvTarefasConcluidas.Columns["DiasReais"].DefaultCellStyle.Alignment =
                gvTarefasConcluidas.Columns["DiferencaDias"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;

                // Destaque para diferenças positivas/negativas
                gvTarefasConcluidas.Columns["DiferencaDias"].DefaultCellStyle.ForeColor = Color.Black;
                gvTarefasConcluidas.CellFormatting += (sender, e) =>
                {
                    if (e.ColumnIndex == gvTarefasConcluidas.Columns["DiferencaDias"].Index && e.Value != null)
                    {
                        if (e.Value.ToString().StartsWith("+"))
                        {
                            e.CellStyle.ForeColor = Color.Red; // Atraso
                        }
                        else if (e.Value.ToString().StartsWith("-"))
                        {
                            e.CellStyle.ForeColor = Color.Green; // Adiantado
                        }
                    }
                };

                gvTarefasConcluidas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void btFechar_Click(object sender, EventArgs e)
        {
            // Fecha o formulário corretamente
            this.Close();

            var kanbanForm = Application.OpenForms.OfType<frmKanban>().FirstOrDefault();
            kanbanForm.Show();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            // Implementação para exportar os dados (exemplo: CSV, Excel)
            MessageBox.Show("Funcionalidade de exportação será implementada em breve.", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}