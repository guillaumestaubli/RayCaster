using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

using RayCaster.Utils;
using RayCaster.Ressources.Enums;

namespace RayCaster
{
    public static class Config
    {
        public const RunMode RUN_MODE = RunMode.DEBUG;

        public const uint FRAME_DURATION = 30; // [ms]
        public const uint BLOC_SIZE = 10; // [px]

        public const string ARIAL_FONT_PATH = @"Ressources\Fonts\arial.ttf";

        // Map config
        public const string MAP_PATH = @"Ressources\World\map.bmp";
        //public const string MAP_PATH = @"Ressources\World\map_test.bmp";

        // 2D Render config
        public static readonly Color WIN_2D_CLEAR_COLOR = new Color(150, 150, 150);
        public static readonly Color WALLS_2D_RENDER_COLOR = new Color(0, 255, 0);

        // 3D Window config
        public const int WIN3D_WIDTH = 800; // [px]
        public const int WIN3D_HEIGHT = 400; // [px]
        public static readonly Color WIN_3D_CLEAR_COLOR = new Color(150, 150, 150);
        public static readonly Color WALLS_3D_RENDER_COLOR = new Color(0, 255, 0);

        // 3D Render config
        public static readonly Color COLOR_WALLS_3D_NORTH = new Color(255, 0, 0);
        public static readonly Color COLOR_WALLS_3D_SOUTH = new Color(0, 255, 0);
        public static readonly Color COLOR_WALLS_3D_WEST = new Color(0, 0, 255);
        public static readonly Color COLOR_WALLS_3D_EAST = new Color(255, 255, 0);

        public static readonly Color COLOR_WALLS_3D_CORNER = new Color(0, 0, 0);
        public static readonly Color COLOR_WALLS_3D_BORDER = new Color(0, 0, 0);

        public static readonly Color COLOR_SKY = new Color(13, 142, 222);
        public static readonly Color COLOR_GROUND = new Color(1, 84, 0);

        public const string WALL_GRAPHICS = @"Ressources\Graphics\wall_test.png";
        public const string NPC_GRAPHICS = @"Ressources\Graphics\npc.png";

        // Player config
        public const int PLAYER_SPAWN_X = 500; // [px]
        public const int PLAYER_SPAWN_Y = 100; // [px]
        public const int PLAYER_RADIUS = (int)BLOC_SIZE / 2; // [px]
        public const double PLAYER_MOVE_SPEED = 2.5f; // [px / frame]
        public const double PLAYER_SPRINT_MULT = 2.0f;
        /// <summary> Rotation speed in Radians </summary>
        public static readonly double PLAYER_ROTATE_SPEED = MathExt.DegToRad(5.0f); // [deg] -> [rad]
        public const double PLAYER_FOV = 60.0f; // [deg]

        // RayCaster config
        public const double RAY_DIFF_ANGLE = PLAYER_FOV / WIN3D_WIDTH;
        public const double RAY_MAX_LENGTH = 500;
        public static readonly Color FOV_RAY_COLOR = new Color(0, 0, 0, 50);
        public const double CORNER_SENSITIVITY = 0.01; // [px]

        // Environement config
        public static VertexArray VERTICES_GROUND = new VertexArray(PrimitiveType.Quads, 4);
        public static VertexArray VERTICES_SKY = new VertexArray(PrimitiveType.Quads, 4);

    }
}
