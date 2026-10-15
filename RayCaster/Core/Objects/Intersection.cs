using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RayCaster;
using RayCaster.Core;
using RayCaster.Core.Objects;
using RayCaster.Entities;
using RayCaster.Ressources;
using RayCaster.Ressources.Enums;
using RayCaster.Utils;

namespace RayCaster.Core.Objects
{
    public class Intersection
    {
        public Vector2d Startpoint { get; set; } = new Vector2d(-1.0, -1.0);
        public Vector2d Endpoint { get; set; } = new Vector2d(-1.0, -1.0);
        public Segment Segment { get; set; } = new Segment();
        public Bloc Bloc { get; set; } = new Bloc();

        public Intersection()
        {

        }

        public Intersection(Vector2d startpoint, Vector2d endpoint, Segment segment, Bloc bloc)
        {
            Startpoint = startpoint;
            Endpoint = endpoint;
            Segment = segment;
            Bloc = bloc;
        }

        public double Length
        {
            get
            {
                return MathExt.GetLength(Startpoint, Endpoint);
            }
        }
    }
}
