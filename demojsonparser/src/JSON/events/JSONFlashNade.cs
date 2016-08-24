using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using demojsonparser.src.JSON.objects;

namespace demojsonparser.src.JSON.events
{
    class JSONFlashNade : JSONNade
    {
        public IList<JSONPlayer> flashedplayers { get; set; }
    }
}
