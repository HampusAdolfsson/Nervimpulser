using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ArmWrestling.Components
{
    class Line
    {
        const float ImageThickness = 26;


        public Vector2 Pos1;
        public float Height { get; set; }
        public float Thickness { get; set; }
        public Texture2D Light { get; private set; }
        public Texture2D Glow { get; private set; }

        private GameMain.GetWindowSizeDelegate windowSizeDelegate;
        private Random random;
        private float randThickness;
        private float thicknessScale;
        private int thickness_update_time;

        private bool starting;
        private bool stopping;
        private bool running;
        private int elapsed_time;

        private float alpha = 0f;

        public Line(float thickness, GameMain.GetWindowSizeDelegate windowSizeDelegate)
        {
            Thickness = thickness;
            randThickness = thickness;
            this.windowSizeDelegate = windowSizeDelegate;
            Vector2 windowSize = windowSizeDelegate();
            Pos1 = new Vector2(windowSize.X / 2, 0);
            random = new Random();
        }

        public void LoadContent(ContentManager cntMgr)
        {
            Light = cntMgr.Load<Texture2D>("line_light");
            Glow = cntMgr.Load<Texture2D>("burn_line");
        }

        public void Update(GameTime gameTime, float standing)
        {
            Vector2 windowSize = windowSizeDelegate();
            Pos1.X = windowSize.X * standing;
            Height = windowSize.Y;

            if (starting)
            {
                elapsed_time += gameTime.ElapsedGameTime.Milliseconds;
                alpha += gameTime.ElapsedGameTime.Milliseconds / 750f;

                if (elapsed_time >= 750)
                {
                    starting = false;
                }
            } else if (stopping)
            {
                elapsed_time += gameTime.ElapsedGameTime.Milliseconds;
                alpha -= gameTime.ElapsedGameTime.Milliseconds / 300f;
                
                if (elapsed_time >= 300)
                {
                    stopping = false;
                }
            } else if (running)
            {
                thickness_update_time += gameTime.ElapsedGameTime.Milliseconds;
                randThickness = Thickness + (float) Math.Sin(thickness_update_time * Math.PI/1000) * Thickness / 8;
                alpha = 0.95f + (float)Math.Sin(thickness_update_time * Math.PI / 1500 + 350) * 0.05f;
                if (thickness_update_time >= 3000) thickness_update_time = 0;
            }

            thicknessScale = randThickness / ImageThickness;
        }

        public void DrawNormal(SpriteBatch spriteBatch)
        {
            Vector2 middleOrigin = new Vector2(0, Glow.Height / 2f);
            Vector2 middleScale = new Vector2(Height, thicknessScale);

            spriteBatch.Draw(Glow , Pos1, null, Color.White * alpha, MathHelper.Pi / 2, middleOrigin, middleScale, SpriteEffects.None, 0f);
        }

        public void DrawLight(SpriteBatch spriteBatch)
        {
            Vector2 middleOrigin = new Vector2(0, Light.Height / 2f);
            Vector2 middleScale = new Vector2(Height, thicknessScale);

            spriteBatch.Draw(Light, Pos1, null, Color.White * alpha, MathHelper.Pi / 2, middleOrigin, middleScale, SpriteEffects.None, 0f);
        }

        public void Start()
        {
            starting = true;
            running = true;
            elapsed_time = 0;
        }

        public void End()
        {
            stopping = true;
            running = false;
            elapsed_time = 0;
        }

    }
}
