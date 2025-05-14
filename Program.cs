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
            using (var db = new iTask())
            {
                if (!db.Utilizadores.Any(u => u.Username == "Gestor"))
                {
                    var gestor = new Gestor
                    {
                        Nome = "Gestor",
                        Username = "Gestor",
                        Password = "1234",
                        TipoUtilizador = "Gestor",
                        Departamento = "IT",
                        GereUtilizadores = "Sim"
                    };

                    db.Utilizadores.Add(gestor);
                    db.SaveChanges();
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmLogin());
        }
    }
}