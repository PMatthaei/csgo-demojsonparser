using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON.Entities;

namespace JSON.Events
{
    class PlayerHurt : WeaponFire
    {
        public Player attacker { get; set; }
        public Player victim { get; set; }
        public int HP { get; set; }
        public int armor { get; set; }
        public int armor_damage { get; set; }
        public int HP_damage { get; set; }
        public int hitgroup { get; set; }
    }
}
