using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace ArmWrestling.Components.TopLevel
{
    class GameScreen
    {
        public event EventHandler PlayerWon;

        private DataHandler dataHandler;
        private ParticleEngine particleEngine;
        private Line line; 

        private Texture2D left, right;
        private Rectangle left_source_rect, right_source_rect;
        private GameMain.GetWindowSizeDelegate windowSizeDelegate;
        private GameMain.GetScaleDelegate scaleDelegate;

        public bool isRunning { get; private set; }

        private Vector2 pos_left, pos_right;
        private float scale_left, scale_right;

        public GameScreen(GameMain.GetWindowSizeDelegate windowSizeDelegate, GameMain.GetScaleDelegate scaleDelegate)
        {
            this.windowSizeDelegate = windowSizeDelegate;
            this.scaleDelegate = scaleDelegate;
            dataHandler = new DataHandler();
            line = new Line(50, windowSizeDelegate);
        }

        public void LoadContent(ContentManager content)
        {
            left = content.Load<Texture2D>("wrestling_sprite_blue");
            right = content.Load<Texture2D>("wrestling_sprite_red");

            List<Texture2D> textures = new List<Texture2D>();
            textures.Add(content.Load<Texture2D>("particle"));
            var lightTexture = content.Load<Texture2D>("particle_light");
            Texture2D particle_light = content.Load<Texture2D>("particle_light");
            particleEngine = new ParticleEngine(textures, particle_light, windowSizeDelegate, scaleDelegate);
            line.LoadContent(content);
        }

        public void Start()
        {
            isRunning = true;
            particleEngine.SpawnsParticles = true;
            line.Start();
            dataHandler.Reset();
        }

        public void Reset()
        {
            isRunning = false;
            particleEngine.SpawnsParticles = false;
            line.End();
            dataHandler.Reset();
        }

        public void Update(GameTime gameTime)
        {
            if (isRunning)
            {
                dataHandler.Update(gameTime);
                if (dataHandler.Standing >= 1 || dataHandler.Standing <= 0)
                {
                    isRunning = false;
                    particleEngine.SpawnsParticles = false;
                    line.End();
                    PlayerWon(this, new PlayerWonEventArgs(dataHandler.Standing >= 1));
                }
            }
            particleEngine.Update(dataHandler.Standing, dataHandler.GetDiff());
            line.Update(gameTime, dataHandler.Standing);

            Vector2 window = windowSizeDelegate();
            left_source_rect = new Rectangle(0, 0, (int)Math.Min(left.Width, left.Height * window.X / window.Y), (int)Math.Min(left.Height, left.Width * window.Y / window.X));
            scale_left = Math.Min(window.X / left_source_rect.Width, window.Y / left_source_rect.Height);

            right_source_rect = new Rectangle(0, 0, (int)Math.Min(right.Width, right.Height * window.X / window.Y), (int)Math.Min(right.Height, right.Width * window.Y / window.X));
            scale_right = Math.Min(window.X / right_source_rect.Width, window.Y / right_source_rect.Height);

            pos_left = new Vector2(window.X * (dataHandler.Standing-1), 0) + dataHandler.LeftDisplacement;
            pos_right = new Vector2(window.X * dataHandler.Standing, 0) + dataHandler.RightDisplacement;
        }

        public void DrawNormal(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(left, pos_left, left_source_rect, Color.White, 0, Vector2.Zero, scale_left, SpriteEffects.None, 0);
            spriteBatch.Draw(right, pos_right, right_source_rect, Color.White, 0, Vector2.Zero, scale_right, SpriteEffects.None, 0);

            particleEngine.DrawNormal(spriteBatch);
            line.DrawNormal(spriteBatch);
        }

        public void DrawLight(SpriteBatch spriteBatch)
        {
            particleEngine.DrawLight(spriteBatch);
            //line.DrawLight(spriteBatch);
        }

        public class PlayerWonEventArgs : EventArgs
        {
            public bool leftWon;
            public PlayerWonEventArgs(Boolean leftWon)
            {
                this.leftWon = leftWon;
            }
        }

    }
}
