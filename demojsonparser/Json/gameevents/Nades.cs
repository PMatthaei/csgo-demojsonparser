using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON.Entities;

namespace JSON.Events
{
    /// <summary>
    /// JSON holding all information needed(supplied) for every nadetype (except flashbangs see JSONFlashNade)
    /// </summary>
    class Nades : Gameevent
    {
        public string nadetype { get; set; }
        public Player thrownby { get; set; }
        public Position3D position { get; set; }
    }

    class FlashNade : Nades
    {
        public IList<FlashedPlayer> flashedplayers { get; set; }
    }
}
