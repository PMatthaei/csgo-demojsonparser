using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON.Entities;

namespace JSON.Events
{
    /// <summary>
    /// Event describing where a player saw another player of the opposing team
    /// </summary>
    public class PlayerSpotted : Gameevent
    {
        public Player spotter { get; set; } //TODO: how find out spotter? or do this in algorithm?
    }
}
