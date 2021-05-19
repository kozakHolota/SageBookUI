using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SageBookUI.DataModels
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public ICollection<Sage> Sages { get; set; }

        public Book() { Sages = new List<Sage>(); }

        public override string ToString() => Title;
    }
}
