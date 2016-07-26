using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using demojsonparser.src.JSON.objects;

namespace demojsonparser.src.JSON.events
{
    class JSONBombEvents
    {
        public char site { get; set; }
        public JSONPlayer player { get; set; }
        public bool haskit { get; set; }
    }
}
