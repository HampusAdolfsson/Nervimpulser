using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ArmWrestling.Components.TopLevel
{
    class GameScreen
    {
        public event EventHandler PlayerWon;

        private ParticleEngine particleEngine;
        private Texture2D left, right;
        private Rectangle left_source_rect, right_source_rect;
        private Vector2 window;

        public bool isRunning { get; private set; }
        private DataHandler dataHandler;

        private Vector2 pos_left, pos_right;
        private float scale_left, scale_right;

        public GameScreen(ref Vector2 windowSize, Texture2D left, Texture2D right, List<Texture2D> particles, Process inputProcess)
        {
            this.left = left;
            this.right = right;
            window = windowSize;
            particleEngine = new ParticleEngine(particles, ref windowSize);
            dataHandler = new DataHandler(inputProcess);

            left_source_rect = new Rectangle(0, 0, (int) Math.Min(left.Width, left.Height * 16f / 9), (int) Math.Min(left.Height, left.Width * 9f / 16));
            scale_left = Math.Min(window.X / left_source_rect.Width, window.Y / left_source_rect.Height);
            right_source_rect = new Rectangle(0, 0, (int) Math.Min(right.Width, right.Height * 16f / 9), (int) Math.Min(right.Height, right.Width * 9f / 16));
            scale_right = Math.Min(window.X / right_source_rect.Width, window.Y / right_source_rect.Height);
        }

        public void Start()
        {
            isRunning = true;
            particleEngine.SpawnsParticles = true;
            dataHandler.Reset();
        }

        public void Reset()
        {
            isRunning = false;
            particleEngine.SpawnsParticles = false;
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
                    PlayerWon(this, new PlayerWonEventArgs(dataHandler.Standing >= 1));
                }
            }
            particleEngine.Update(dataHandler.Standing, dataHandler.GetDiff());

            pos_left = new Vector2(window.X * (dataHandler.Standing-1), 0) + dataHandler.LeftDisplacement;
            pos_right = new Vector2(window.X * dataHandler.Standing, 0) + dataHandler.RightDisplacement;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(left, pos_left, left_source_rect, Color.White, 0, Vector2.Zero, scale_left, SpriteEffects.None, 0);
            spriteBatch.Draw(right, pos_right, right_source_rect, Color.White, 0, Vector2.Zero, scale_right, SpriteEffects.None, 0);

            particleEngine.Draw(spriteBatch);
            spriteBatch.End();
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
