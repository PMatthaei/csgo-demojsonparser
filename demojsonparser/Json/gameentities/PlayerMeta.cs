using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSON.Entities
{
    public class PlayerMeta
    {
        public string playername { get; set; }
        public string team { get; set; }
        public string clanname { get; set; }
        public long steam_id { get; set; }
    }
}
