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
        RenderTarget2D mainTarget;
        RenderTarget2D lightMap;

        Process inputProcess;

        //content
        SpriteFont countDownFont;
        SpriteFont fpsFont;
        Effect lightingEffect;

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

        SamplerState _clampTextureAddressMode;

        public GameMain()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferWidth = 1920;

        }

        protected override void Initialize()
        {
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

            windowSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            scale = new Vector2(windowSize.X / 800, windowSize.Y / 450);

            var pp = GraphicsDevice.PresentationParameters;
            mainTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            lightMap = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);

            // initialize components
            GetWindowSizeDelegate windowSizeDelegate = new GetWindowSizeDelegate(() => { return windowSize; });
            GetScaleDelegate scaleDelegate = new GetScaleDelegate(() => { return scale; });
            gameScreen = new GameScreen(windowSizeDelegate, scaleDelegate, inputProcess);
            gameScreen.PlayerWon += (s, e) => { textHandler.ShowVictoryScreen(((GameScreen.PlayerWonEventArgs)e).leftWon); };
            textHandler = new TextHandler(windowSizeDelegate);
            textHandler.CountdownFinished += (s, e) => { gameScreen.Start(); };
            textHandler.VictoryScreenFinished += (s, e) => { gameScreen.Reset(); };
            textHandler.StartWait();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            lightingEffect = Content.Load<Effect>("lighting");
            
            fpsFont = Content.Load<SpriteFont>("fps");

            textHandler.LoadContent(Content);
            gameScreen.LoadContent(Content);
            
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
                if (!graphics.IsFullScreen) {
                    graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                } else
                {
                    graphics.PreferredBackBufferWidth = 800;
                    graphics.PreferredBackBufferHeight = 450;
                }
                var pp = GraphicsDevice.PresentationParameters;
                mainTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
                lightMap = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);

                graphics.ToggleFullScreen();
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
            GraphicsDevice.SetRenderTarget(lightMap);
            GraphicsDevice.Clear(Color.White * 0.3f);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            spriteBatch.DrawString(fpsFont, "FPS:" + _fps, new Vector2(), Color.White);
            gameScreen.DrawLight(spriteBatch);
            textHandler.Draw(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(mainTarget);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            gameScreen.DrawNormal(spriteBatch);
            textHandler.Draw(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            lightingEffect.Parameters["lightMap"].SetValue(lightMap);
            lightingEffect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(mainTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            _total_frames++;

            base.Draw(gameTime);
        }

        public delegate Vector2 GetWindowSizeDelegate();
        public delegate Vector2 GetScaleDelegate();
    }
}
