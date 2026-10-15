using RayCaster.Core.Objects;
using RayCaster.Entities;
using RayCaster.Ressources.Enums;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayCaster.Core
{
    public class Renderer3D
    {
        private RayCaster _rayCaster;
        private World _world;

        private double _max_length;

        private List<Vertex> _vertices_sky = new List<Vertex>();
        private List<Vertex> _vertices_walls = new List<Vertex>();
        private List<Vertex> _vertices_ground = new List<Vertex>();

        public Renderer3D(ref RayCaster rayCaster, ref World world)
        {
            _rayCaster = rayCaster;
            _world = world;

            _max_length = Config.RAY_MAX_LENGTH;
        }

        public VertexArray VerticesSky
        {
            get
            {
                VertexArray array = new VertexArray(PrimitiveType.Quads, (uint)_vertices_sky.Count);

                for (int i = 0; i < _vertices_sky.Count; i++)
                    array[(uint)i] = _vertices_sky[i];

                return array;
            }
        }

        public VertexArray VerticesWalls
        {
            get
            {
                VertexArray array = new VertexArray(PrimitiveType.Quads, (uint)_vertices_walls.Count);

                for (int i = 0; i < _vertices_walls.Count; i++)
                    array[(uint)i] = _vertices_walls[i];

                return array;
            }
        }

        public VertexArray VerticesGround
        {
            get
            {
                VertexArray array = new VertexArray(PrimitiveType.Quads, (uint)_vertices_ground.Count);

                for (int i = 0; i < _vertices_ground.Count; i++)
                    array[(uint)i] = _vertices_ground[i];

                return array;
            }
        }

        public void Generate()
        {
            _vertices_sky = new List<Vertex>();
            _vertices_walls = new List<Vertex>();
            _vertices_ground = new List<Vertex>();

            List<RaysBySegment> raysBySegments = new List<RaysBySegment>();
            int pos_x = 0;

            foreach (Ray ray in _rayCaster.Rays)
            {
                RaysBySegment? raysBySegment = raysBySegments.Where(x => x.Segment.ID == ray.Segment.ID).FirstOrDefault();

                if (raysBySegment == null)
                {
                    raysBySegments.Add(new RaysBySegment
                    {
                        Segment = ray.Segment,
                        Rays = new List<Ray>() { ray }
                    });

                    raysBySegment = raysBySegments.Last();
                }
                else
                {
                    raysBySegment.Rays.Add(ray);
                }

                List<Vertex> vertices = GetVerticesForRay(ray, pos_x);

                raysBySegment.VerticesByRays.Add(new VerticesByRay
                {
                    Ray = ray,
                    Vertices = vertices
                });

                pos_x++;
            }

            List<Vertex> final_vertices = new List<Vertex>();

            foreach (RaysBySegment raysBySegment in raysBySegments)
            {
                List<Vertex> firstVertices = raysBySegment.VerticesByRays.First().Vertices;
                List<Vertex> lastVertices = raysBySegment.VerticesByRays.Last().Vertices;

                List<Vertex> vertices_sky =
                [
                    firstVertices[0],
                    firstVertices[1],
                    lastVertices[1],
                    lastVertices[0]
                ];

                List<Vertex> vertices_wall =
                [
                    firstVertices[2],
                    firstVertices[3],
                    lastVertices[3],
                    lastVertices[2]
                ];

                List<Vertex> vertices_ground =
                [
                    firstVertices[4],
                    firstVertices[5],
                    lastVertices[5],
                    lastVertices[4]
                ];

                _vertices_sky.AddRange(CorrectBlankSpacesBetweenQuads(vertices_sky));
                _vertices_walls.AddRange(CorrectBlankSpacesBetweenQuads(vertices_wall));
                _vertices_ground.AddRange(CorrectBlankSpacesBetweenQuads(vertices_ground));
            }
        }

        private List<Vertex> GetVerticesForRay(Ray ray, int pos_x)
        {
            List<Vertex> vertices = new List<Vertex>();

            double length = ray.RayLength * Math.Cos(ray.RotationRelativeFov);

            double ratio = 1.0 - (length / _max_length);

            if (length > _max_length)
                ratio = 0;

            double height = Config.WIN3D_HEIGHT * ratio;
            double space_y = (Config.WIN3D_HEIGHT - height) / 2.0;

            height = Math.Round(height);
            space_y = Math.Round(space_y);

            byte alpha_variation = (byte)(255.0 * ratio);

            Color color_wall = Color.White;
            Color color_sky = Config.COLOR_SKY;
            Color color_ground = Config.COLOR_GROUND;

            switch (ray.BlocSideIntersected)
            {
                case Direction.NORTH:
                    color_wall = Config.COLOR_WALLS_3D_NORTH;
                    break;
                case Direction.SOUTH:
                    color_wall = Config.COLOR_WALLS_3D_SOUTH;
                    break;
                case Direction.WEST:
                    color_wall = Config.COLOR_WALLS_3D_WEST;
                    break;
                case Direction.EAST:
                    color_wall = Config.COLOR_WALLS_3D_EAST;
                    break;
                case Direction.NORTH_WEST:
                case Direction.NORTH_EAST:
                case Direction.SOUTH_WEST:
                case Direction.SOUTH_EAST:
                    color_wall = Config.COLOR_WALLS_3D_CORNER;
                    break;
            }

            color_wall.A = alpha_variation;

            // sky
            vertices.Add(new Vertex(new Vector2f(Config.WIN3D_WIDTH - pos_x, 0.0f), color_sky));
            color_sky.A = alpha_variation;
            vertices.Add(new Vertex(new Vector2f(Config.WIN3D_WIDTH - pos_x, (float)space_y), color_sky));

            // wall
            vertices.Add(new Vertex(new Vector2f(Config.WIN3D_WIDTH - pos_x, (float)space_y), color_wall));
            vertices.Add(new Vertex(new Vector2f(Config.WIN3D_WIDTH - pos_x, (float)(space_y + height)), color_wall));

            // ground
            color_ground.A = alpha_variation;
            vertices.Add(new Vertex(new Vector2f(Config.WIN3D_WIDTH - pos_x, (float)(space_y + height)), color_ground));
            color_ground.A = 255;
            vertices.Add(new Vertex(new Vector2f(Config.WIN3D_WIDTH - pos_x, (float)Config.WIN3D_HEIGHT), color_ground));

            return vertices;
        }

        private List<Vertex> CorrectBlankSpacesBetweenQuads(List<Vertex> vertices)
        {
            Vertex top_right = vertices[0];
            Vertex bottom_right = vertices[1];
            Vertex bottom_left = vertices[2];
            Vertex top_left = vertices[3];

            if (top_right.Position.X != 0 && top_right.Position.X != Config.WIN3D_WIDTH)
            {
                top_right.Position.X += 0.5f;
            }

            if (bottom_right.Position.X != 0 && bottom_right.Position.X != Config.WIN3D_WIDTH)
            {
                bottom_right.Position.X += 0.5f;
            }

            if (bottom_left.Position.X != 0 && bottom_left.Position.X != Config.WIN3D_WIDTH)
            {
                bottom_left.Position.X -= 0.5f;
            }

            if (top_left.Position.X != 0 && top_left.Position.X != Config.WIN3D_WIDTH)
            {
                top_left.Position.X -= 0.5f;
            }

            return [top_right, bottom_right, bottom_left, top_left];
        }
    }

    public class RaysBySegment
    {
        public Segment Segment { get; set; } = new Segment();

        public List<Ray> Rays { get; set; } = new List<Ray>();

        public List<VerticesByRay> VerticesByRays { get; set; } = new List<VerticesByRay>();
    }

    public class VerticesByRay
    {
        public Ray Ray { get; set; } = new Ray();

        public List<Vertex> Vertices { get; set; } = new List<Vertex>();
    }
}
