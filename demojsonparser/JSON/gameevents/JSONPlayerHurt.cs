using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON.Objects;

namespace JSON.Events
{
    class JSONPlayerHurt : JSONGameevent
    {
        public JSONPlayer attacker { get; set; }
        public JSONPlayer victim { get; set; }
        public int HP { get; set; }
        public int armor { get; set; }
        public int armor_damage { get; set; }
        public int HP_damage { get; set; }
        public string hitgroup { get; set; }
        public JSONItem weapon { get; set; }
    }
}
