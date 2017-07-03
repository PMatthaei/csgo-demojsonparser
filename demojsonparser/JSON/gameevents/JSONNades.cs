using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON.Objects;

namespace JSON.Events
{
    /// <summary>
    /// JSON holding all information needed(supplied) for every nadetype (except flashbangs see JSONFlashNade)
    /// </summary>
    class JSONNade : JSONGameevent
    {
        public string nadetype { get; set; }
        public JSONPlayer thrownby { get; set; }
        public JSONPosition3D position { get; set; }
    }
}
