using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using ArmWrestling.Components.TopLevel;

namespace ArmWrestling
{
    public class GameMain : Game
    {
        readonly GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        RenderTarget2D _mainTarget;
        RenderTarget2D _lightMap;

        Process _inputProcess;

        //content
        SpriteFont _fpsFont;
        Effect _lightingEffect;

        //data
        GameScreen _gameScreen;
        TextHandler _textHandler;

        //fps
        int _fps;
        float _elapsedTime;
        int _totalFrames;

        //resolution
        Vector2 _windowSize;
        Vector2 _scale;

        public GameMain()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.PreferredBackBufferWidth = 1920;

        }

        protected override void Initialize()
        {
            _inputProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "java.exe",
                    Arguments = "-jar WrestlingProcessing.jar",
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };

            _inputProcess.Start();

            _windowSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            _scale = new Vector2(_windowSize.X / 800, _windowSize.Y / 450);

            var pp = GraphicsDevice.PresentationParameters;
            _mainTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            _lightMap = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);

            // initialize components
            var windowSizeDelegate = new GetWindowSizeDelegate(() => _windowSize);
            var scaleDelegate = new GetScaleDelegate(() => _scale);
            _gameScreen = new GameScreen(windowSizeDelegate, scaleDelegate);
            _gameScreen.PlayerWon += (s, e) => { _textHandler.ShowVictoryScreen(((GameScreen.PlayerWonEventArgs)e).LeftWon); };
            _textHandler = new TextHandler(windowSizeDelegate);
            _textHandler.CountdownFinished += (s, e) => { _gameScreen.Start(); };
            _textHandler.VictoryScreenFinished += (s, e) => { _gameScreen.Reset(); };
            _textHandler.StartWait();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _lightingEffect = Content.Load<Effect>("lighting");
            
            _fpsFont = Content.Load<SpriteFont>("fps");

            _textHandler.LoadContent(Content);
            _gameScreen.LoadContent(Content);
            
        }
        

        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Escape))
            {
                if (!_inputProcess.HasExited) _inputProcess.Kill();
                Exit();
                Environment.Exit(0);
                
            }
            if (state.IsKeyDown(Keys.F11))
            {
                if (!_graphics.IsFullScreen) {
                    _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                } else
                {
                    _graphics.PreferredBackBufferWidth = 800;
                    _graphics.PreferredBackBufferHeight = 450;
                }
                var pp = GraphicsDevice.PresentationParameters;
                _mainTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
                _lightMap = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);

                _graphics.ToggleFullScreen();
                _graphics.ApplyChanges();
            }

            _elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
            if (_elapsedTime > 1000)
            {
                _elapsedTime -= 1000;
                _fps = _totalFrames;
                _totalFrames = 0;
            }
            _windowSize.X = GraphicsDevice.Viewport.Width;
            _windowSize.Y = GraphicsDevice.Viewport.Height;
            _scale.X = _windowSize.X / 800;
            _scale.Y = _windowSize.Y / 450;
            _textHandler.Update(gameTime);
            _gameScreen.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_lightMap);
            GraphicsDevice.Clear(Color.White * 0.3f);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            _spriteBatch.DrawString(_fpsFont, "FPS:" + _fps, new Vector2(), Color.White);
            _gameScreen.DrawLight(_spriteBatch);
            _textHandler.Draw(_spriteBatch);
            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(_mainTarget);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            _gameScreen.DrawNormal(_spriteBatch);
            _textHandler.Draw(_spriteBatch);
            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            _lightingEffect.Parameters["lightMap"].SetValue(_lightMap);
            _lightingEffect.CurrentTechnique.Passes[0].Apply();
            _spriteBatch.Draw(_mainTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();

            _totalFrames++;

            base.Draw(gameTime);
        }

        public delegate Vector2 GetWindowSizeDelegate();
        public delegate Vector2 GetScaleDelegate();
    }
}
