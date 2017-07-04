using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON.Entities;

namespace JSON.Events
{
    public class ServerEvents : Gameevent
    {
        /// <summary>
        /// Type of the server event (Disconnect, Connect etc)
        /// </summary>
        public string servereventtype { get; set; }
    }

    public class TakeOverEvent : ServerEvents
    {
        /// <summary>
        /// The player which is been taken over by a bot
        /// </summary>
        public Player taken;
    }
}
