using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demojsonparser.src.JSON.objects
{
    class JSONGamestate : IDisposable
    {
        public JSONGamemeta meta { get; set; }
        public JSONMatch match { get; set; }

        public void Dispose()
        {
            if (match != null && meta != null)
            {
                match = null;
                meta = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();

            }
        }
    }
}
