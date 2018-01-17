using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Final.QuickNote.Class
{
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public byte[] Document { get; set; }
        public DateTime LastModified { get; set; }
        public ColorTag ColorNote { get; set; }
    }
}