using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON.Entities;

namespace JSON.Events
{
    class WeaponFire : Gameevent
    {
        public Player shooter { get; set; }
        public Item weapon { get; set; }
    }
}
