using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;

using RayCaster.Ressources.Enums;
using RayCaster.Utils;
using System.Diagnostics;
using SFML.Graphics;
using RayCaster.Core.Objects;

namespace RayCaster.Entities
{
    public class Ray
    {
        public int ID { get; }

        public Vector2d Origin { get; set; }
        public Vector2d Destination { get; set; }
        public double Rotation { get; set; }
        public double RotationRelativeFov { get; set; }

        private bool _intersectionDetected = false;
        private Bloc _blocIntersected = new Bloc();
        private Direction _blocSideIntersected = Direction.NONE;
        private Vector2d _intersectedPoint = new Vector2d(-1.0f, -1.0f);
        private double _rayLength = -1.0f;
        private Segment _segment = new Segment();

        public bool IntersectionDetected { get { return _intersectionDetected; } }
        public Bloc BlocIntersected { get { return _blocIntersected; } }
        public Direction BlocSideIntersected { get { return _blocSideIntersected; } }
        public Vector2d IntersectedPoint { get { return _intersectedPoint; } }
        public double RayLength { get { return _rayLength; } }
        public Segment Segment { get { return _segment; } }

        public double Length
        {
            get
            {
                return MathExt.GetLength(Origin, Destination);
            }
        }

        public Vector2d Step
        {
            get
            {
                return new Vector2d(Math.Cos(Rotation), Math.Sin(Rotation));
            }
        }

        public Ray()
        {
            ID = int.MinValue;
            Origin = new Vector2d(double.MinValue, double.MinValue);
            Destination = new Vector2d(double.MinValue, double.MinValue);
            Rotation = double.MinValue;
            RotationRelativeFov = double.MinValue;
            _intersectionDetected = false;
            _blocIntersected = new Bloc();
            _blocSideIntersected = Direction.NONE;
            _intersectedPoint = new Vector2d(double.MinValue, double.MinValue);
            _rayLength = double.MinValue;
        }

        public Ray(int id, Vector2d origin, Vector2d destination, double rotation, double rotationRelativeFov)
        {
            ID = id;

            Origin = origin;
            Destination = destination;
            Rotation = rotation;
            RotationRelativeFov = rotationRelativeFov;

            _intersectionDetected = false;
            _blocIntersected = new Bloc();
            _blocSideIntersected = Direction.NONE;
            _intersectedPoint = new Vector2d(-1.0f, -1.0f);
        }

        public void Reset(Vector2d origin, Vector2d destination, double rotation, double rotationRelativeFov)
        {
            Origin = origin;
            Destination = destination;
            Rotation = rotation;
            RotationRelativeFov = rotationRelativeFov;

            _intersectionDetected = false;
            _blocIntersected = new Bloc();
            _blocSideIntersected = Direction.NONE;
            _intersectedPoint = new Vector2d(-1.0f, -1.0f);
            _rayLength = -1.0f;
        }

        public void SetIntersection(Intersection intersection)
        {
            Destination = intersection.Endpoint;
            _blocIntersected = intersection.Bloc;
            _blocSideIntersected = intersection.Segment.Exposed_Side;
            _intersectionDetected = true;
            _rayLength = intersection.Length;
            _intersectedPoint = intersection.Endpoint;
            _segment = intersection.Segment;
        }
    }
}
