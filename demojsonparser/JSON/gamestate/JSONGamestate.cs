using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSON
{
    public class JSONGamestate
    {
        /// <summary>
        /// Meta data about the game/match
        /// </summary>
        public JSONGamemeta meta { get; set; }

        /// <summary>
        /// The raw match data with events and playerdata
        /// </summary>
        public JSONMatch match { get; set; }
    }
}
