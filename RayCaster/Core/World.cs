using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RayCaster.Entities;
using RayCaster.Ressources.Enums;
using RayCaster.Utils;
using RayCaster.Core.Objects;

using SFML.Graphics;
using SFML.System;

namespace RayCaster.Core
{
    public class World
    {
        public List<Bloc> WorldBlocs { get; }
        public Bloc[,] MappedBlocs { get; }
        public List<Segment> Segments { get; }

        public uint Width { get; } = 0;
        public uint Height { get; } = 0;

        public World(string path)
        {
            Image imgMap = new Image(path);

            Width = imgMap.Size.X;
            Height = imgMap.Size.Y;

            WorldBlocs = new List<Bloc>();
            MappedBlocs = new Bloc[Width, Height];

            uint blocId = 1;

            for (uint i = 0; i < Width; ++i)
            {
                for (uint j = 0; j < Height; ++j)
                {
                    if (
                        imgMap.GetPixel(i, j).R == 0 &&
                        imgMap.GetPixel(i, j).G == 255 &&
                        imgMap.GetPixel(i, j).B == 0)
                    {
                        MappedBlocs[i, j] = new Bloc(blocId, (int)i, (int)j, BlocType.WALL);
                        WorldBlocs.Add(MappedBlocs[i, j]);
                    }
                    else
                    {
                        MappedBlocs[i, j] = new Bloc(blocId, (int)i, (int)j, BlocType.EMPTY);
                    }

                    blocId++;
                }
            }

            Segments = GenerateSegments();

            Segments = Segments.OrderBy(x => x.Endpoint.X).ThenBy(x => x.Endpoint.Y).ToList();

            Segments = GroupSegments(Segments);
        }

        private List<Segment> GenerateSegments()
        {
            List<Segment> segments = new List<Segment>();

            List<Direction> directions = new List<Direction>() { Direction.NORTH, Direction.SOUTH, Direction.WEST, Direction.EAST };
            double b_size = Config.BLOC_SIZE;

            int counter = 0;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Bloc bloc = MappedBlocs[x, y];

                    if (bloc.Type == BlocType.WALL)
                    {
                        foreach (Direction dir in directions)
                        {
                            counter++;

                            Bloc? nextBloc = GetBloc(bloc, dir);

                            if (nextBloc != null && nextBloc.Type == BlocType.EMPTY)
                            {
                                Segment segment = new Segment();
                                segment.Direction = dir == Direction.NORTH || dir == Direction.SOUTH ? Direction.HORIZONTAL : Direction.VERTICAL;
                                segment.Exposed_Side = dir;

                                switch (dir)
                                {
                                    case Direction.NORTH:
                                        segment.Startpoint = bloc.WindowPos + new Vector2d(0.0, 0.0);
                                        segment.Endpoint = bloc.WindowPos + new Vector2d(b_size, 0.0);
                                        break;
                                    case Direction.SOUTH:
                                        segment.Startpoint = bloc.WindowPos + new Vector2d(0.0, b_size);
                                        segment.Endpoint = bloc.WindowPos + new Vector2d(b_size, b_size);
                                        break;
                                    case Direction.WEST:
                                        segment.Startpoint = bloc.WindowPos + new Vector2d(0.0, 0.0);
                                        segment.Endpoint = bloc.WindowPos + new Vector2d(0.0, b_size);
                                        break;
                                    case Direction.EAST:
                                        segment.Startpoint = bloc.WindowPos + new Vector2d(b_size, 0.0);
                                        segment.Endpoint = bloc.WindowPos + new Vector2d(b_size, b_size);
                                        break;
                                }

                                segments.Add(segment);
                            } // endif

                        } // endforeach

                    } // endif

                } // endfor

            } // endfor

            return segments;
        }

        private List<Segment> GroupSegments(List<Segment> segments)
        {
            List<Segment> groupped = new List<Segment>();

            foreach (Segment segment in segments)
            {
                Segment? group = groupped.Where(x => x.Endpoint == segment.Startpoint && x.Exposed_Side == segment.Exposed_Side).FirstOrDefault();

                if (group != null)
                {
                    group.Endpoint = segment.Endpoint;
                }
                else
                {
                    group = new Segment()
                    {
                        Startpoint = segment.Startpoint,
                        Endpoint = segment.Endpoint,
                        Exposed_Side = segment.Exposed_Side,
                        Direction = segment.Direction
                    };

                    groupped.Add(group);
                }
            }

            for (int i = 0; i < groupped.Count; i++)
            {
                groupped[i].ID = (uint)i + 1;
            }

            return groupped;
        }

        private Bloc? GetBloc(Bloc origin, Direction direction)
        {
            int x = origin.GridPos.X;
            int y = origin.GridPos.Y;

            switch (direction)
            {
                case Direction.NORTH:
                    y--;
                    break;
                case Direction.SOUTH:
                    y++;
                    break;
                case Direction.WEST:
                    x--;
                    break;
                case Direction.EAST:
                    x++;
                    break;
            }

            if (0 <= x && x < Width && 0 <= y && y < Height)
                return MappedBlocs[x, y];
            else
                return null;
        }

        public Bloc? GetBloc(Vector2d coords)
        {
            coords = MathExt.RoundCoords(coords);

            double x = coords.X;
            double y = coords.Y;

            int grid_x = (int)coords.X / (int)Config.BLOC_SIZE;
            int grid_y = (int)coords.Y / (int)Config.BLOC_SIZE;

            return GetBloc(grid_x, grid_y);
        }

        public Bloc? GetBloc(int grid_x, int grid_y)
        {
            if (grid_x >= 0 && grid_x < MappedBlocs.GetLength(0) && grid_y >= 0 && grid_y < MappedBlocs.GetLength(1))
            {
                return MappedBlocs[grid_x, grid_y];
            }
            else
            {
                return null;
            }
        }

    }


}
