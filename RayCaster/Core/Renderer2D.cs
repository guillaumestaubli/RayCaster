using RayCaster.Entities;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayCaster.Core
{
    public class Renderer2D
    {
        private RayCaster _rayCaster;

        private VertexArray _vertices;

        public Renderer2D(ref RayCaster rayCaster)
        {
            _rayCaster = rayCaster;

            _vertices = new VertexArray(PrimitiveType.Lines, (uint)_rayCaster.Rays.Count * 2);
        }

        public VertexArray Vertices
        {
            get
            {
                return _vertices;
            }
        }

        public void Generate()
        {
            _vertices.Dispose();

            _vertices = new VertexArray(PrimitiveType.Lines, (uint)_rayCaster.Rays.Count * 2);

            for (int i = 0; i < _rayCaster.Rays.Count; i++)
            {
                _vertices[(uint)i * 2] = new Vertex((Vector2f)_rayCaster.Rays[i].Origin, Config.FOV_RAY_COLOR);
                _vertices[((uint)i * 2) + 1] = new Vertex((Vector2f)_rayCaster.Rays[i].Destination, Config.FOV_RAY_COLOR);
            }
        }
    }
}
