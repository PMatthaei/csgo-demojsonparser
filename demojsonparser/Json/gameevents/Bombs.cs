using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON.Entities;

namespace JSON.Events
{
    class Bombs : Gameevent
    {
        /// <summary>
        /// The site the bomb was planted
        /// </summary>
        public char site { get; set; }

        /// <summary>
        /// The Player planting the bomb or defusing it
        /// </summary>
        public Player player { get; set; }

        /// <summary>
        /// If the player has a defuser
        /// </summary>
        public bool haskit { get; set; }
    }

    public class BombState : Gameevent
    {
        public Player player { get; set; }
    }
}
