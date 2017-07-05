using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JSON
{
    /// <summary>
    /// Class holding every data to parse as well as information about how to parse
    /// </summary>
    public class ParseTask
    {

        /// <summary>
        /// Source of the demo file
        /// </summary>
        public string srcpath { get; set; }

        /// <summary>
        /// Destination of the JSON file
        /// </summary>
        public string destpath { get; set; }

        /// <summary>
        /// Pretty print json
        /// </summary>
        public bool usepretty { get; set; }

        /// <summary>
        /// Disable positional updates just show events
        /// </summary>
        public bool showsteps { get; set; }

        /// <summary>
        /// Interval at which positions should be updated. Increase filesize if fast interval!!
        /// </summary>
        public int positioninterval { get; set; }

        /// <summary>
        /// Just show specical events
        /// </summary>
        public bool specialevents { get; set; }

        /// <summary>
        /// Use highly detailed player data for ouput. Increase filesize!!
        /// </summary>
        public bool highdetailplayer { get; set; }

        /// <summary>
        /// Serialiser Settings mainly for deserialising the file into 
        /// </summary>
        public JsonSerializerSettings settings { get; set; }

    }
}
