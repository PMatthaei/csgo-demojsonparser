using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSON.Entities
{
    public class Player
    {
        public bool isSpotted;
        public string playername { get; set; }
        public long player_id { get; set; }
        public string team { get; set; }
        public Position3D position { get; set; }
        public Facing facing { get; set; }
        public Velocity velocity { get; set; }
        public int HP { get; internal set; }
    }

    public class FlashedPlayer : Player
    {
        public float flashedduration { get; set; }
    }


    public class Position3D
    {
        public Position3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class Facing
    {
        public float Yaw { get; set; }
        public float Pitch { get; set; }
    }

    public class Velocity
    {
        public float VX { get; set; }
        public float VY { get; set; }
        public float VZ { get; set; }
    }
}
