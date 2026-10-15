using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RayCaster.Ressources.Enums;
using RayCaster.Utils;

using SFML.Graphics;
using SFML.System;

namespace RayCaster.Entities
{
    public class Player
    {
        /// <summary>
        /// Orientation of the FOV in Rad
        /// </summary>
        public double Rotation { get; set; }
        public Vector2d Position { get; set; }

        public CircleShape Body { get; set; }
        public RectangleShape Eye { get; set; }

        public FloatRect GlobalBounds
        {
            get
            {
                return Body.GetGlobalBounds();
            }
        }

        public Player()
        {
            Rotation = 0.0;

            Position = new Vector2d(Config.PLAYER_SPAWN_X, Config.PLAYER_SPAWN_Y);

            Body = new CircleShape(Config.PLAYER_RADIUS);
            Body.Position = (Vector2f)Position;
            Body.Origin = new Vector2f(Config.PLAYER_RADIUS, Config.PLAYER_RADIUS);
            Body.FillColor = Color.Red;
            Body.Rotation = (float)Rotation;

            Eye = new RectangleShape(new Vector2f(Config.PLAYER_RADIUS, 2.0f));
            Eye.Position = (Vector2f)Position;
            Eye.Origin = new Vector2f(0.0f, 1.0f);
            Eye.FillColor = Color.White;
            Eye.Rotation = (float)Rotation;
        }

        public void Rotate(Direction dir)
        {
            if (dir == Direction.LEFT)
                Rotation -= Config.PLAYER_ROTATE_SPEED;
            else
                Rotation += Config.PLAYER_ROTATE_SPEED;

            Rotation = MathExt.CorrectRotationMultiples(Rotation);

            Body.Rotation = (float)MathExt.RadToDeg(Rotation);
            Eye.Rotation = (float)MathExt.RadToDeg(Rotation);
        }

        public void Move(Direction dir, bool sprint = false)
        {
            double moveX;
            double moveY;

            if (sprint)
            {
                moveX = Config.PLAYER_MOVE_SPEED * Config.PLAYER_SPRINT_MULT * (double)Math.Cos(Rotation);
                moveY = Config.PLAYER_MOVE_SPEED * Config.PLAYER_SPRINT_MULT * (double)Math.Sin(Rotation);
            }
            else
            {
                moveX = Config.PLAYER_MOVE_SPEED * (double)Math.Cos(Rotation);
                moveY = Config.PLAYER_MOVE_SPEED * (double)Math.Sin(Rotation);
            }

            Vector2d move = new Vector2d(moveX, moveY);

            if (dir == Direction.BACKWARD)
            {
                move *= (-1.0f);
                Position -= new Vector2d(moveX, moveY);
            }
            else
            {
                Position += new Vector2d(moveX, moveY);
            }

            Body.Position = (Vector2f)Position;
            Eye.Position = (Vector2f)Position;
        }

        public void MoveX(double delta)
        {
            double x = Position.X + delta;
            double y = Position.Y;

            Position = new Vector2d(x, y);

            Body.Position = (Vector2f)Position;
            Eye.Position = (Vector2f)Position;
        }

        public void MoveY(double delta)
        {
            double x = Position.X;
            double y = Position.Y + delta;

            Position = new Vector2d(x, y);

            Body.Position = (Vector2f)Position;
            Eye.Position = (Vector2f)Position;
        }

    }
}
