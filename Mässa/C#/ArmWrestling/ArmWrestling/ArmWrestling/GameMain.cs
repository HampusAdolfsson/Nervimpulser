using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using ArmWrestling.Components;
using ArmWrestling.Components.TopLevel;

namespace ArmWrestling
{
    public class GameMain : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Process inputProcess;

        //sprites
        SpriteFont countDownFont;
        SpriteFont fpsFont;

        //data
        GameScreen gameScreen;
        TextHandler textHandler;

        //fps
        int _fps = 0;
        float _elapsed_time = 0f;
        int _total_frames = 0;

        //resolution
        Vector2 windowSize;
        Vector2 scale;

        public GameMain()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 450;
            graphics.PreferredBackBufferWidth = 800;

        }

        protected override void Initialize()
        {
            windowSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            scale = new Vector2(windowSize.X / 800, windowSize.Y / 450);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            countDownFont = Content.Load<SpriteFont>("CountDown");
            fpsFont = Content.Load<SpriteFont>("fps");

            List<Texture2D> textures = new List<Texture2D>();
            textures.Add(Content.Load<Texture2D>("particle"));
            
            var left = Content.Load<Texture2D>("blue");
            var right = Content.Load<Texture2D>("red");

            inputProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "java.exe",
                    Arguments = "-jar WrestlingProcessing.jar",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };

            inputProcess.Start();

            GetWindowSizeDelegate windowSizeDelegate = new GetWindowSizeDelegate(() => { return windowSize; });
            GetScaleDelegate scaleDelegate = new GetScaleDelegate(() => { return scale; });
            gameScreen = new GameScreen(windowSizeDelegate, scaleDelegate, left, right, textures, inputProcess);
            gameScreen.PlayerWon += (s, e) => { textHandler.ShowVictoryScreen(((GameScreen.PlayerWonEventArgs)e).leftWon); };
            textHandler = new TextHandler(countDownFont, windowSizeDelegate);
            textHandler.CountdownFinished += (s, e) => { gameScreen.Start(); };
            textHandler.VictoryScreenFinished += (s, e) => { gameScreen.Reset(); };
            textHandler.StartWait();
        }
        

        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Escape))
            {
                if (!inputProcess.HasExited) inputProcess.Kill();
                Exit();
                Environment.Exit(0);
                
            }
            if (state.IsKeyDown(Keys.F11))
            {
                graphics.ToggleFullScreen();
                if (graphics.IsFullScreen) {
                    graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                } else
                {
                    graphics.PreferredBackBufferWidth = 800;
                    graphics.PreferredBackBufferHeight = 450;
                }
                graphics.ApplyChanges();
            }

            _elapsed_time += gameTime.ElapsedGameTime.Milliseconds;
            if (_elapsed_time > 1000)
            {
                _elapsed_time -= 1000;
                _fps = _total_frames;
                _total_frames = 0;
            }
            windowSize.X = GraphicsDevice.Viewport.Width;
            windowSize.Y = GraphicsDevice.Viewport.Height;
            scale.X = windowSize.X / 800;
            scale.Y = windowSize.Y / 450;
            textHandler.Update(gameTime);
            gameScreen.Update(gameTime);

            base.Update(gameTime);
        }
        

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            gameScreen.Draw(spriteBatch);
            
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.DrawString(fpsFont, "FPS:" + _fps, new Vector2(), Color.White);
            textHandler.Draw(spriteBatch);
            spriteBatch.End();

            _total_frames++;

            base.Draw(gameTime);
        }
        public delegate Vector2 GetWindowSizeDelegate();
        public delegate Vector2 GetScaleDelegate();
    }
}
