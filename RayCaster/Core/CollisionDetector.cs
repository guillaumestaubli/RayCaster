using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

using RayCaster.Entities;
using RayCaster.Ressources.Enums;
using RayCaster.Utils;

namespace RayCaster.Core
{
    class CollisionDetector
    {
        private World _world;
        private Player _player;

        public CollisionDetector(ref World world, ref Player player)
        {
            _world = world;
            _player = player;
        }

        public void DetectAndCorrectCollisions()
        {
            FloatRect playerGlobalBounds = _player.GlobalBounds;

            Vector2i currentGridPos = new Vector2i((int)(_player.Position.X / Config.BLOC_SIZE), (int)(_player.Position.Y / Config.BLOC_SIZE));

            for (int d_x = -2; d_x <= 2; ++d_x)
            {
                for (int d_y = -2; d_y <= 2; ++d_y)
                {
                    if (currentGridPos.X + d_x >= 0 && currentGridPos.X + d_x < _world.Width && currentGridPos.Y + d_y >= 0 && currentGridPos.Y + d_y < _world.Height)
                    {
                        Bloc bloc = _world.MappedBlocs[currentGridPos.X + d_x, currentGridPos.Y + d_y];

                        if (bloc.Type != BlocType.EMPTY)
                        {
                            if (playerGlobalBounds.Intersects(bloc.Bounds))
                            {
                                Border border = new Border();

                                if (PlayerIntersects(bloc, ref border))
                                {
                                    double delta = Config.PLAYER_RADIUS - border.Delta;

                                    switch (border.Direction)
                                    {
                                        case Direction.FORWARD:
                                            _player.MoveY((-1.0) * delta);
                                            break;
                                        case Direction.BACKWARD:
                                            _player.MoveY(delta);
                                            break;
                                        case Direction.LEFT:
                                            _player.MoveX((-1.0) * delta);
                                            break;
                                        case Direction.RIGHT:
                                            _player.MoveX(delta);
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool PlayerIntersects(Bloc bloc, ref Border returnBorder)
        {
            Vector2d bpos = bloc.WindowPos;
            Vector2d ppos = _player.Position;

            List<Border> borders = new List<Border>
            {
                new Border(new Vector2d(bpos.X + Config.BLOC_SIZE, bpos.Y), Direction.FORWARD),
                new Border(new Vector2d(bpos.X, bpos.Y + Config.BLOC_SIZE), Direction.BACKWARD),
                new Border(new Vector2d(bpos.X, bpos.Y), Direction.LEFT),
                new Border(new Vector2d(bpos.X + Config.BLOC_SIZE, bpos.Y + Config.BLOC_SIZE), Direction.RIGHT)
            };

            for (int i = 0; i < Config.BLOC_SIZE; ++i)
            {
                foreach (Border border in borders)
                {
                    border.TempDelta = GetDelta(ppos, border.Point);

                    if (border.TempDelta < Config.PLAYER_RADIUS)
                    {
                        border.Collision = true;
                        if (border.TempDelta < border.Delta)
                        {
                            border.Delta = border.TempDelta;
                        }
                    }

                    border.NextPoint();
                }
            }

            List<Border> collisions = new List<Border>();

            foreach (Border border in borders)
            {
                if (border.Collision)
                    collisions.Add(border);
            }

            if (collisions.Count > 0)
            {
                returnBorder = collisions.OrderBy(x => x.Delta).First();

                return true;
            }

            return false;
        }

        private double GetDelta(Vector2d point_a, Vector2d point_b)
        {
            double delta_x = Math.Abs(point_a.X - point_b.X);
            double delta_y = Math.Abs(point_a.Y - point_b.Y);

            return Math.Sqrt(Math.Pow(delta_x, 2.0) + Math.Pow(delta_y, 2.0));
        }
    }

    class Border
    {
        public Direction Direction { get; set; } = Direction.NONE;

        public Vector2d Point { get; set; }

        public double Delta { get; set; } = Config.PLAYER_RADIUS;
        public double TempDelta { get; set; } = 0.0;

        public bool Collision { get; set; } = false;

        public Border()
        {

        }

        public Border(Vector2d point, Direction direction)
        {
            Direction = direction;
            Point = point;
        }

        public void NextPoint()
        {
            double x = Point.X;
            double y = Point.Y;

            switch (Direction)
            {
                case Direction.FORWARD:
                    x -= 1;
                    break;
                case Direction.BACKWARD:
                    x += 1;
                    break;
                case Direction.LEFT:
                    y += 1;
                    break;
                case Direction.RIGHT:
                    y -= 1;
                    break;
            }

            Point = new Vector2d(x, y);
        }
    }
}
