using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON.Entities;

namespace JSON.Events
{
    class PlayerMovement : Gameevent
    {
        public DetailedPlayer player { get; set; }
    }
}
