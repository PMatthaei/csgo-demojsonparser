using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON.Objects;

namespace JSON.Events
{
    class JSONPlayerFootstep : JSONGameevent
    {
        public JSONPlayer player { get; set; }
    }
}
