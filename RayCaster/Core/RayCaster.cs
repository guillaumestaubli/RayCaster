using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Diagnostics;

using SFML.System;
using SFML.Window;
using SFML.Graphics;

using Newtonsoft.Json;

using RayCaster.Entities;
using RayCaster.Utils;
using RayCaster.Ressources.Enums;
using RayCaster.Core.Objects;
using Microsoft.VisualBasic;
using System.Reflection.Metadata;

namespace RayCaster.Core
{
    public class RayCaster
    {
        private World _world;
        private Player _player;

        public uint RaysLost { get; set; } = 0;

        public List<Ray> Rays { get; set; }

        public RayCaster(ref World world, ref Player player)
        {
            _world = world;
            _player = player;

            Rays = new List<Ray>();

            double lowerBound = -1.0 * Config.PLAYER_FOV / 2.0;
            double upperBound = Config.PLAYER_FOV / 2.0;

            int id = 0;

            // create all the lines used to check intersections
            int nbBloc_X = _world.MappedBlocs.GetLength(0);
            int nbBloc_Y = _world.MappedBlocs.GetLength(1);

            // create all the rays to cast through the world
            for (double i = lowerBound; i < upperBound; i += Config.RAY_DIFF_ANGLE)
            {
                Rays.Add(new Ray(id, _player.Position, _player.Position, _player.Rotation, 0.0));

                id += 1;
            }
        }

        public void CastRays()
        {
            RaysLost = 0;

            double lowerBound = -1.0 * Config.PLAYER_FOV / 2.0; // [deg]
            double upperBound = Config.PLAYER_FOV / 2.0; // [deg]

            int rayIndex = 0;
            double i_rad;
            double rayRotation;
            Intersection? intersection;
            Vector2d step = new Vector2d();

            for (double i = lowerBound; i <= upperBound; i += Config.RAY_DIFF_ANGLE)
            {
                Ray ray = Rays[rayIndex];

                i_rad = MathExt.DegToRad(i);

                rayRotation = MathExt.CorrectRotationMultiples(_player.Rotation - i_rad);
                rayRotation = MathExt.CorrectPerfectAngles(rayRotation);

                step.X = Math.Cos(rayRotation);
                step.Y = Math.Sin(rayRotation);

                Rays[rayIndex].Reset(_player.Position, _player.Position + step, rayRotation, i_rad);

                List<Intersection> intersections = GetIntersections(Rays[rayIndex]);

                if (intersections.Count == 0)
                    RaysLost++;

                intersection = GetNearestIntersection(intersections);

                if (intersection == null)
                {
                    intersection = new Intersection()
                    {
                        Startpoint = _player.Position,
                        Endpoint = new Vector2d(Config.WIN3D_WIDTH, Config.WIN3D_HEIGHT),
                        Segment = new Segment()
                        {
                            Exposed_Side = Direction.NONE
                        }
                    };
                }

                Rays[rayIndex].SetIntersection(intersection);

                ++rayIndex;
            }
        }

        private List<Intersection> GetIntersections(Ray ray, bool debug = false)
        {
            List<Segment> fltrdSegments = GetIntersectionSegments(ray.Origin, ray.Rotation);
            List<Intersection> intersections = new List<Intersection>();

            foreach (Segment segment in fltrdSegments)
            {
                Vector2d point = MathExt.GetIntersection(ray.Origin, ray.Destination, segment.Startpoint, segment.Endpoint);

                point = MathExt.RoundCoords(point);

                bool insideWorldBounds = IsIntersectionInsideWorld(point);

                if (insideWorldBounds)
                {
                    bool inFrontOfThePlayer = IsPointInFrontOfTheRay(ray.Origin, point, ray);

                    if (inFrontOfThePlayer)
                    {
                        bool insideSegmentBounds = IsOnTheSegment(point, segment);

                        if (insideSegmentBounds)
                        {
                            Bloc? bloc_before_step = _world.GetBloc(point);
                            Bloc? bloc_after_step = _world.GetBloc(point + ray.Step);

                            Bloc bloc = new Bloc();

                            if (bloc_before_step != null && bloc_after_step != null)
                            {
                                if (bloc_before_step.Id != bloc_after_step.Id && bloc_before_step.Type == bloc_after_step.Type)
                                    bloc = bloc_before_step;
                                else
                                    bloc = bloc_after_step;

                                if (bloc.Type == BlocType.WALL)
                                {
                                    Intersection intersection = new Intersection()
                                    {
                                        Startpoint = ray.Origin,
                                        Endpoint = point,
                                        Segment = segment,
                                        Bloc = bloc
                                    };

                                    intersections.Add(intersection);
                                }
                            }
                        }
                    }
                }
            }

            return intersections;
        }

        private Intersection? GetNearestIntersection(List<Intersection> intersections)
        {
            Intersection? nearest = intersections.OrderBy(x => x.Length).FirstOrDefault();

            return nearest;
        }

        private List<Segment> GetIntersectionSegments(Vector2d origin, double angle)
        {
            List<Segment> segments = new List<Segment>();

            angle = MathExt.CorrectRotationMultiples(angle);

            // 0° -> 90° & -360° -> -270°
            if ((MathExt.RAD_0 < angle && angle < MathExt.RAD_90) || (-1.0 * MathExt.RAD_360 < angle && angle < -1.0 * MathExt.RAD_270))
            {
                segments.AddRange(_world.Segments.Where(x => x.Direction == Direction.HORIZONTAL && x.Startpoint.Y >= origin.Y).ToList());
                segments.AddRange(_world.Segments.Where(x => x.Direction == Direction.VERTICAL && x.Startpoint.X >= origin.X).ToList());
            }

            // 90° -> 180° & -270° -> -180°
            else if ((MathExt.RAD_90 < angle && angle < MathExt.RAD_180) || (-1.0 * MathExt.RAD_270 < angle && angle < -1.0 * MathExt.RAD_180))
            {
                segments.AddRange(_world.Segments.Where(x => x.Direction == Direction.HORIZONTAL && x.Startpoint.Y >= origin.Y).ToList());
                segments.AddRange(_world.Segments.Where(x => x.Direction == Direction.VERTICAL && x.Startpoint.X <= origin.X).ToList());
            }

            // 180° -> 270° & -180° -> -90°
            else if ((MathExt.RAD_180 < angle && angle < MathExt.RAD_270) || (-1.0 * MathExt.RAD_180 < angle && angle < -1.0 * MathExt.RAD_90))
            {
                segments.AddRange(_world.Segments.Where(x => x.Direction == Direction.HORIZONTAL && x.Startpoint.Y <= origin.Y).ToList());
                segments.AddRange(_world.Segments.Where(x => x.Direction == Direction.VERTICAL && x.Startpoint.X <= origin.X).ToList());
            }

            // 270° -> 360° & -90° -> -0°
            else if ((MathExt.RAD_270 < angle && angle < MathExt.RAD_360) || (-1.0 * MathExt.RAD_90 < angle && angle < -1.0 * MathExt.RAD_0))
            {
                segments.AddRange(_world.Segments.Where(x => x.Direction == Direction.HORIZONTAL && x.Startpoint.Y <= origin.Y).ToList());
                segments.AddRange(_world.Segments.Where(x => x.Direction == Direction.VERTICAL && x.Startpoint.X >= origin.X).ToList());
            }

            return segments.Count > 0 ? segments : _world.Segments;
        }

        private bool IsIntersectionInsideWorld(Vector2d point)
        {
            double x_min = Config.BLOC_SIZE;
            double x_max = (_world.Width * Config.BLOC_SIZE) - Config.BLOC_SIZE;

            double y_min = Config.BLOC_SIZE;
            double y_max = (_world.Height * Config.BLOC_SIZE) - Config.BLOC_SIZE;

            return x_min <= point.X && point.X <= x_max && y_min <= point.Y && point.Y <= y_max;
        }

        private bool IsPointInFrontOfTheRay(Vector2d origin, Vector2d point, Ray ray)
        {
            double angle = MathExt.CorrectRotationMultiples(ray.Rotation);
            angle = MathExt.RadToDeg(angle);
            angle = (float)Math.Round(angle, 3);
            angle = MathExt.DegToRad(angle);

            // 0° & 360° & -0° & -360°
            if (angle == MathExt.RAD_0 || angle == MathExt.RAD_360 || angle == -1.0 * MathExt.RAD_0 || angle == -1.0 * MathExt.RAD_360)
            {
                return point.X > origin.X && point.Y == origin.Y;
            }

            // 90° & -270°
            else if (angle == MathExt.RAD_90 || angle == -1.0 * MathExt.RAD_270)
            {
                return point.X == origin.X && point.Y > origin.Y;
            }

            // 180° & -180°
            else if (angle == MathExt.RAD_180 || angle == -1.0 * MathExt.RAD_180)
            {
                return point.X < origin.X && point.Y == origin.Y;
            }

            // 270° & -90°
            else if (angle == MathExt.RAD_270 || angle == -1.0 * MathExt.RAD_90)
            {
                return point.X == origin.X && point.Y < origin.Y;
            }

            // 0° -> 90° & -360° -> -270°
            else if ((MathExt.RAD_0 < angle && angle < MathExt.RAD_90) || (-1.0 * MathExt.RAD_360 < angle && angle < -1.0 * MathExt.RAD_270))
            {
                return point.X > origin.X && point.Y > origin.Y;
            }

            // 90° -> 180° & -270° -> -180°
            else if ((MathExt.RAD_90 < angle && angle < MathExt.RAD_180) || (-1.0 * MathExt.RAD_270 < angle && angle < -1.0 * MathExt.RAD_180))
            {
                return point.X < origin.X && point.Y > origin.Y;
            }

            // 180° -> 270° & -180° -> -90°
            else if ((MathExt.RAD_180 < angle && angle < MathExt.RAD_270) || (-1.0 * MathExt.RAD_180 < angle && angle < -1.0 * MathExt.RAD_90))
            {
                return point.X < origin.X && point.Y < origin.Y;
            }

            // 270° -> 360° & -90° -> -0°
            else if ((MathExt.RAD_270 < angle && angle < MathExt.RAD_360) || (-1.0 * MathExt.RAD_90 < angle && angle < -1.0 * MathExt.RAD_0))
            {
                return point.X > origin.X && point.Y < origin.Y;
            }

            return false;
        }

        private bool IsOnTheSegment(Vector2d point, Segment segment)
        {
            double x0 = segment.Startpoint.X < segment.Endpoint.X ? segment.Startpoint.X : segment.Endpoint.X;
            double x1 = segment.Startpoint.X < segment.Endpoint.X ? segment.Endpoint.X : segment.Startpoint.X;

            double y0 = segment.Startpoint.Y < segment.Endpoint.Y ? segment.Startpoint.Y : segment.Endpoint.Y;
            double y1 = segment.Startpoint.Y < segment.Endpoint.Y ? segment.Endpoint.Y : segment.Startpoint.Y;

            double x = point.X;
            double y = point.Y;

            return x0 <= x && x <= x1 && y0 <= y && y <= y1;
        }
    }
}
