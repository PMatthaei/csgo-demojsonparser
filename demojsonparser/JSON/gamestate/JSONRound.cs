using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSON
{
    public class JSONRound
    {
        /// <summary>
        /// ID of the round i.e. the number of the round
        /// </summary>
        public int round_id { get; set; }

        /// <summary>
        /// Winner of this round
        /// </summary>
        public string winner { get; set; }

        /// <summary>
        /// Ticks occuring in this round
        /// </summary>
        public List<JSONTick> ticks { get; set; }

    }
}
