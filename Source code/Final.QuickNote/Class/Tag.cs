using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Final.QuickNote
{
    public class Tag
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; } = 0;
        [Ignore]
        public bool IsSelected { get; set; } = false;
    }
}
