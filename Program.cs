using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace iTasks
{
    internal static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var db = new Utilizadores())
            {
                var cliente = new Cliente { Nome = "Cliente1", Nif = "123456789" };
                db.Clientes.Add(cliente);
                var carro = new Carro { Matricula = "AB-01-AA", Cliente = cliente };
                db.Carro.Add(carro);
                var servico = new Servico { Tipo = "Revisão", Data = DateTime.Now, Carro = carro };
                db.Servicos.Add(servico);
                var servico2 = new Servico { Tipo = "Revisão 2", Data = DateTime.Now, Carro = carro };
                db.Servicos.Add(servico);
                var parcela = new Parcela { Valor = 100, Descricao = "Ta bom", Servico = servico };
                db.Parcelas.Add(parcela);
                var parcela2 = new Parcela { Valor = 200, Descricao = "Ta bom 2", Servico = servico };
                db.Parcelas.Add(parcela);
                db.SaveChanges();
            }
        }
    }
}
