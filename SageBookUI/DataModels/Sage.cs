using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SageBookUI.DataModels
{
    public class Sage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string City { get; set; }
        public ICollection<Book> Books { get; set; }

        public Sage() { Books = new List<Book>(); }

        public override string ToString() => Name;
    }
}
