using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON.Entities;

namespace JSON.Events
{
    public class Gameevent
    {
        /// <summary>
        /// The gameevent identifier as string
        /// </summary>
        public string gameeventtype { get; set; }

        /// <summary>
        /// The player causing this event
        /// </summary>
        public Player actor { get; set; }

    }
}
