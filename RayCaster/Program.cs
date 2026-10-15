using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using RayCaster.Core;
using RayCaster.Ressources.Enums;
using RayCaster.Utils;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace RayCaster
{
    internal class Program
    {
        static void Main(string[] args)
        {
            switch (Config.RUN_MODE)
            {
                case RunMode.DEBUG:
                    RunDebug();
                    break;
                case RunMode.RELEASE:
                    RunRelease();
                    break;
            }

            if (Logger.LogsWritten)
            {
                DirectoryInfo dir = new DirectoryInfo("logs");

                Process.Start("explorer.exe", dir.FullName);
            }
        }

        private static void RunDebug()
        {
            MathExt.Init();

            Game game = new Game();

            game.Run();
        }

        private static void RunRelease()
        {
            try
            {
                MathExt.Init();

                Game game = new Game();

                game.Run();
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogLevel.ERROR, "A fatal error occured", $"{nameof(Program)} - {nameof(Main)}", ex);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n--------------------------------------------------------------");
                Console.WriteLine("> A FATAL ERROR OCCURED. CHECK LOGS FILES FOR MORE INFOS");
                Console.WriteLine("--------------------------------------------------------------");

                Console.ForegroundColor = ConsoleColor.Gray;

                Console.WriteLine();
                Console.WriteLine(ex.ToString());
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------------------");
            }
        }

        private static void TEST()
        {
            VideoMode vm = new VideoMode(800, 400);

            RenderWindow window = new RenderWindow(vm, "TEST", Styles.Close);

            while (true)
            {
                window.DispatchEvents();

                window.Clear(Config.WIN_3D_CLEAR_COLOR);

                VertexArray vertexArray = new VertexArray(PrimitiveType.Quads, 8);

                Vertex v_0 = new Vertex(new Vector2f(10, 10), new Vector2f(0, 0));
                Vertex v_1 = new Vertex(new Vector2f(10, 110), new Vector2f(0, 800));
                Vertex v_2 = new Vertex(new Vector2f(110, 110), new Vector2f(800, 800));
                Vertex v_3 = new Vertex(new Vector2f(110, 10), new Vector2f(800, 0));

                vertexArray[0] = v_0;
                vertexArray[1] = v_1;
                vertexArray[2] = v_2;
                vertexArray[3] = v_3;

                Vertex v_4 = new Vertex(new Vector2f(200, 200), new Vector2f(0, 0));
                Vertex v_5 = new Vertex(new Vector2f(250, 300), new Vector2f(0, 800));
                Vertex v_6 = new Vertex(new Vector2f(300, 300), new Vector2f(800, 800));
                Vertex v_7 = new Vertex(new Vector2f(300, 200), new Vector2f(800, 0));

                vertexArray[4] = v_4;
                vertexArray[5] = v_5;
                vertexArray[6] = v_6;
                vertexArray[7] = v_7;

                Texture texture = new Texture(Config.WALL_GRAPHICS);

                RenderStates state = new RenderStates(texture);

                window.Draw(vertexArray, state);

                window.Display();
            }
        }
    }
}
