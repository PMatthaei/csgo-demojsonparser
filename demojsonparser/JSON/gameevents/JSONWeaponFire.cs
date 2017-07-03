using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON.Objects;

namespace JSON.Events
{
    class JSONWeaponFire : JSONGameevent
    {
        public JSONPlayerDetailed shooter { get; set; }
        public JSONItem weapon { get; set; }
    }
}
