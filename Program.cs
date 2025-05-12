using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iTasks
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Criação do contexto da base de dados
            using (var db = new iTask())
            {
                // Verifica se já existe um utilizador com o mesmo username
                if (!db.Utilizadores.Any(u => u.Username == "Programador"))
                {
                    // Cria um novo utilizador do tipo "Programador"
                    var programador = new Programador
                    {
                        Nome = "Programador",
                        Username = "Programador",
                        Password = "1234", // Nota: Considerar encriptar esta password
                        TipoUtilizador = "Programador"
                    };

                    // Adiciona o utilizador à base de dados e guarda as alterações
                    db.Utilizadores.Add(programador);
                    db.SaveChanges();
                }
            }

            // Inicialização da interface gráfica (Windows Forms)
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmLogin()); // Inicia o formulário de login
        }
    }
}