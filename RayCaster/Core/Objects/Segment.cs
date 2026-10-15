using RayCaster.Ressources.Enums;
using RayCaster.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayCaster.Core.Objects
{
    public class Segment
    {
        public uint ID { get; set; } = uint.MaxValue;

        public Vector2d Startpoint { get; set; } = new Vector2d(-1.0, -1.0);
        public Vector2d Endpoint { get; set; } = new Vector2d(-1.0, -1.0);

        public Direction Direction { get; set; } = Direction.NONE;
        public Direction Exposed_Side { get; set; } = Direction.NONE;
    }
}
