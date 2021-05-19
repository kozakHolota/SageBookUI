using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SageBookUI
{
    public class SgDbContext : DbContext
    {
        public SgDbContext(): base("conStr")
        {
        }
    }
}
