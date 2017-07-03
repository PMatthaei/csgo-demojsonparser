using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON.Objects;

namespace JSON
{
    public class JSONGamemeta
    {
        /// <summary>
        /// ID of the gametstate if needed for DB applications
        /// </summary>
        public int gamestate_id { get; set; }

        /// <summary>
        /// Name of the map played in this game
        /// </summary>
        public string mapname { get; set; }

        /// <summary>
        /// Tickrate of the game in hz
        /// </summary>
        public float tickrate { get; set; }

        /// <summary>
        /// All players participating in the game
        /// </summary>
        public List<JSONPlayerMeta> players { get; set; }
    }
}
