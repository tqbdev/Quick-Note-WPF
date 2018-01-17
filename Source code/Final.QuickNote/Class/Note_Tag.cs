using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Final.QuickNote.Class
{
    public class Note_Tag
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int IdTag { get; set; }
        public int IdNote { get; set; }
    }
}
