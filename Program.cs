using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace iTasks
{
    static class Program
    {
        static void Main()
        {
            using (var db = new iTask())
            {
                int contador = db.Utilizadores.Count();
                var utilizador = new Utilizador
                {
                    Id = contador + 1,
                    Nome = "Admin",
                    Username = "Admin",
                    Password = "1234",
                    TipoUtilizador = "Gestor"
                };

                db.Utilizadores.Add(utilizador);
                db.SaveChanges(); // Importante!
            }
        }
    }
}
