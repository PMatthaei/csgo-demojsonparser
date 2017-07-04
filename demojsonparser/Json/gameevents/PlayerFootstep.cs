using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON.Entities;

namespace JSON.Events
{
    class PlayerFootstep : Gameevent
    {
        public Player player { get; set; }
    }
}
