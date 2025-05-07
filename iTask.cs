using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace iTasks
{
    class iTask : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
    }
}
