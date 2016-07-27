using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using demojsonparser.src.JSON.objects;

namespace demojsonparser.src.JSON.events
{
    class JSONPlayerKilled : JSONGameevent
    {
        public JSONPlayerDetailed attacker { get; set; }
        public JSONPlayerDetailed victim { get; set; }
        public bool headhshot { get; set; }
        public int penetrated { get; set; }
        public int hitgroup { get; set; }
    }
}
