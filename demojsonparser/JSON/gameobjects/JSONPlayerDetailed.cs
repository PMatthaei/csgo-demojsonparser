using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSON.Objects
{
    class JSONPlayerDetailed : JSONPlayer
    {
        internal List<JSONItem> items;

        public int HP { get; set; }
        public int armor { get; set; }
        public bool hasHelmet { get; set; }
        public bool hasDefuser { get; set; }
        public bool hasBomb { get; set; }
        public bool isDucking { get; set; }
        public bool isWalking { get; set; }
        public bool isSpotted { get; set; }
        public bool isScoped { get; set; }
        public double velocity { get; set; }
    }
}
