using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using RayCaster.Entities;
using RayCaster.Ressources.Enums;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

using Newtonsoft.Json;
using RayCaster.Utils;

namespace RayCaster.Core
{
    class Game
    {
        private World _world;
        private Player _player;
        private CollisionDetector _collisionDetector;
        private RayCaster _rayCaster;
        private Renderer2D _renderer2D;
        private Renderer3D _renderer3D;

        private RenderWindow _window_2D;
        private RenderWindow _window_3D;
        private RenderWindow _window_debug;

        private Stopwatch _clock;
        private Stopwatch _frameTimer;

        private Font _arial;

        private Texture _texture_wall;
        private RenderStates _renderStates_wall;

        public Game()
        {
            _world = new World(Config.MAP_PATH);
            _player = new Player();

            _collisionDetector = new CollisionDetector(ref _world, ref _player);
            _rayCaster = new RayCaster(ref _world, ref _player);
            _renderer2D = new Renderer2D(ref _rayCaster);
            _renderer3D = new Renderer3D(ref _rayCaster, ref _world);

            _texture_wall = new Texture(Config.WALL_GRAPHICS);
            _texture_wall.Smooth = true;
            _texture_wall.Repeated = true;
            
            _renderStates_wall = new RenderStates(_texture_wall);
            _renderStates_wall.BlendMode = BlendMode.Alpha;
            

            VideoMode vmWindow2D = new VideoMode((uint)(_world.Width * Config.BLOC_SIZE), (uint)(_world.Height * Config.BLOC_SIZE));

            _window_2D = new RenderWindow(vmWindow2D, "C# - SFML - RayCaster - 2D RenderWindow", Styles.Close);
            _window_2D.SetVerticalSyncEnabled(true);
            _window_2D.Closed += CloseRenderWindows;


            VideoMode vmWindow3D = new VideoMode(Config.WIN3D_WIDTH, Config.WIN3D_HEIGHT);

            _window_3D = new RenderWindow(vmWindow3D, "C# - SFML - RayCaster - 3D RenderWindow", Styles.Close);
            _window_3D.SetVerticalSyncEnabled(true);
            _window_3D.Closed += CloseRenderWindows;


            VideoMode vmWindowDebug = new VideoMode(400, 400);

            _window_debug = new RenderWindow(vmWindowDebug, "C# - SFML - RayCaster - Debug", Styles.Close);
            _window_debug.SetVerticalSyncEnabled(true);
            _window_debug.Closed += CloseRenderWindows;

            _clock = new Stopwatch();
            _frameTimer = new Stopwatch();

            _arial = new Font(Config.ARIAL_FONT_PATH);
        }

        public void Run()
        {
            _frameTimer.Start();
            _clock.Start();

            while (_window_2D.IsOpen && _window_3D.IsOpen)
            {
                _window_2D.DispatchEvents();
                _window_3D.DispatchEvents();

                if (_clock.ElapsedMilliseconds >= Config.FRAME_DURATION)
                {
                    ProcessKeyboardInputs();

                    _frameTimer.Restart();

                    _collisionDetector.DetectAndCorrectCollisions();

                    _rayCaster.CastRays();

                    _renderer2D.Generate();
                    _renderer3D.Generate();

                    _clock.Restart();
                }

                Window_2D_DisplayFrame();
                Window_3D_DisplayFrame();
                Window_Debug_DisplayFrame();
            }

            _window_2D.Dispose();
            _window_3D.Dispose();
            _window_debug.Dispose();
        }

        private void CloseRenderWindows(object? sender, EventArgs e)
        {
            _window_2D.Close();
            _window_3D.Close();
            _window_debug.Close();
        }

        private void ProcessKeyboardInputs()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                _player.Rotate(Direction.LEFT);

            if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                _player.Rotate(Direction.RIGHT);

            if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                _player.Move(Direction.FORWARD, Keyboard.IsKeyPressed(Keyboard.Key.LShift));

            if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                _player.Move(Direction.BACKWARD, Keyboard.IsKeyPressed(Keyboard.Key.LShift));
        }

        private void Window_2D_DisplayFrame()
        {
            _window_2D.Clear(Config.WIN_2D_CLEAR_COLOR);

            _world.WorldBlocs.Where(x => x.Type == BlocType.WALL).ToList().ForEach(x => _window_2D.Draw(x.Vertices));

            _window_2D.Draw(_renderer2D.Vertices);

            _window_2D.Draw(_player.Body);
            _window_2D.Draw(_player.Eye);

            _window_2D.Display();
        }

        private void Window_3D_DisplayFrame()
        {
            _window_3D.Clear(Config.WIN_3D_CLEAR_COLOR);

            _window_3D.Draw(_renderer3D.VerticesSky);
            //_window_3D.Draw(_renderer3D.VerticesWalls, _renderStates_wall);
            _window_3D.Draw(_renderer3D.VerticesWalls);
            _window_3D.Draw(_renderer3D.VerticesGround);

            _window_3D.Display();
        }

        private void Window_Debug_DisplayFrame()
        {
            string[] debug =
            [
                $"> Player position : x = {Math.Round(_player.Position.X, 3)}   |   y = {Math.Round(_player.Position.Y, 3)}",
                $"> Player rotation [rad] : {_player.Rotation}",
                $"> Player rotation [rad] : {MathExt.RadToDeg(_player.Rotation)}",
                $"> Elapsed time : {_frameTimer.Elapsed}",
                $"> Nb Rays : {_rayCaster.Rays.Count}",
                $"> Rays lost : {_rayCaster.RaysLost}",
                $"> Vertices to draw 2D : {_renderer2D.Vertices.VertexCount}",
                $"> Vertices to draw 3D : {_renderer3D.VerticesSky.VertexCount + _renderer3D.VerticesWalls.VertexCount + _renderer3D.VerticesGround.VertexCount}"
            ];

            _window_debug.Clear(Color.Black);

            float x = 10;
            float y = 15;

            Text text;

            for (int i = 0; i < debug.Length; i++)
            {
                text = new Text(debug[i], _arial);
                text.Position = new Vector2f(x, 5 + (i * y));
                text.CharacterSize = 12;

                _window_debug.Draw(text);
            }

            _window_debug.Display();
        }
    }
}
