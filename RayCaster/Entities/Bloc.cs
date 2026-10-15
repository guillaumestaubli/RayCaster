using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RayCaster.Ressources.Enums;
using RayCaster.Utils;
using System.Reflection.Metadata;

namespace RayCaster.Entities
{
    public class Bloc
    {
        public uint Id { get; set; }

        public Vector2i GridPos { get; private set; }
        public Vector2d WindowPos { get; private set; }

        public BlocType Type { get; private set; }

        public VertexArray? Vertices { get; }

        public FloatRect Bounds
        {
            get
            {
                return Vertices == null ? new FloatRect() : Vertices.Bounds;
            }
        }

        public Bloc()
        {
            Id = uint.MaxValue;
            GridPos = new Vector2i(int.MinValue, int.MinValue);
            WindowPos = new Vector2d(double.MinValue, double.MinValue);
            Type = BlocType.NONE;
        }

        public Bloc(uint id, int x, int y, BlocType type)
        {
            Id = id;
            GridPos = new Vector2i(x, y);
            WindowPos = new Vector2d(x * Config.BLOC_SIZE, y * Config.BLOC_SIZE);
            Type = type;

            if (Type == BlocType.WALL)
            {
                uint size = Config.BLOC_SIZE;

                Vertices = new VertexArray(PrimitiveType.Quads, 4);
                Vertices[0] = new Vertex(new Vector2f(x * size, y * size), Config.WALLS_2D_RENDER_COLOR);
                Vertices[1] = new Vertex(new Vector2f((x + 1) * size, y * size), Config.WALLS_2D_RENDER_COLOR);
                Vertices[2] = new Vertex(new Vector2f((x + 1) * size, (y + 1) * size), Config.WALLS_2D_RENDER_COLOR);
                Vertices[3] = new Vertex(new Vector2f(x * size, (y + 1) * size), Config.WALLS_2D_RENDER_COLOR);
            }
        }
    }
}
