using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON.Events;

namespace JSON
{
    public class JSONTick
    {
        /// <summary>
        /// ID of the tick. TickIDs may have gaps !
        /// </summary>
        public int tick_id { get; set; }

        /// <summary>
        /// All events occuring in this tick
        /// </summary>
        public List<JSONGameevent> tickevents { get; set; }
    }
}
