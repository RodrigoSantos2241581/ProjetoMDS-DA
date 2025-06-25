using static System.Data.Entity.Infrastructure.Design.Executor;

namespace iTasks.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Utilizadors", "Departamento", c => c.String());
        }

        public override void Down()
        {
            AlterColumn("dbo.Utilizadors", "Departamento", c => c.Int());
        }
    }
}
