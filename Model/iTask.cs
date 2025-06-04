using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace iTasks
{
    public class iTask : DbContext
    {
        public iTask() : base("iTaskDB")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<iTask, Migrations.Configuration>());
        }

        public DbSet<Utilizador> Utilizadores { get; set; }
        public DbSet<Gestor> Gestores { get; set; }
        public DbSet<Programador> Programadores { get; set; }
        public DbSet<TipoTarefa> TiposTarefas { get; set; }
        public DbSet<Tarefa> Tarefas { get; set; }
    

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Evitar múltiplas cascatas: define DeleteBehavior como Restrict
            modelBuilder.Entity<Tarefa>()
                .HasRequired(t => t.Gestor)
                .WithMany()
                .HasForeignKey(t => t.IdGestor)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tarefa>()
                .HasRequired(t => t.Programador)
                .WithMany()
                .HasForeignKey(t => t.IdProgramador)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
