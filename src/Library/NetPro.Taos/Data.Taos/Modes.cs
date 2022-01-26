using System;
using System.Collections.Generic;
using System.Text;

namespace Maikebing.Data.Taos
{
    internal class _SHOWDATABASES
    {
        public string name { get; set; }
        public int tables { get; set; }
        public int rows { get; set; }
        public int ntables { get; set; }
        public string status { get; set; }

        //"created time",
        //"ntables",
        //"vgroups",
        //"replica",
        //"days",
        //"keep1,keep2,keep(D)",
        //"tables",
        //"rows",
        //"cache(b)",
        //"ablocks",
        //"tblocks",
        //"ctime(s)",
        //"clog",
        //"comp",
        //"time precision",
        //"status"
    }
}
