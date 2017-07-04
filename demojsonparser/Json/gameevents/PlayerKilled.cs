using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON.Entities;

namespace JSON.Events
{
    class PlayerKilled : PlayerHurt
    {
        public DetailedPlayer assister { get; set; }
        public bool headshot { get; set; }
        public int penetrated { get; set; }
    }
}
