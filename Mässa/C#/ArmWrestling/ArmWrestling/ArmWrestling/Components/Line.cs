using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ArmWrestling.Components
{
    internal class Line
    {
        private const float _imageThickness = 26;


        public Vector2 Pos1;
        public float Height { get; set; }
        public float Thickness { get; set; }
        public Texture2D Light { get; private set; }
        public Texture2D Glow { get; private set; }

        private readonly GameMain.GetWindowSizeDelegate _windowSizeDelegate;
        private float _randThickness;
        private float _thicknessScale;
        private int _thicknessUpdateTime;

        private bool _starting;
        private bool _stopping;
        private bool _running;
        private int _elapsedTime;

        private float _alpha;

        public Line(float thickness, GameMain.GetWindowSizeDelegate windowSizeDelegate)
        {
            Thickness = thickness;
            _randThickness = thickness;
            this._windowSizeDelegate = windowSizeDelegate;
            Vector2 windowSize = windowSizeDelegate();
            Pos1 = new Vector2(windowSize.X / 2, 0);
        }

        public void LoadContent(ContentManager cntMgr)
        {
            Light = cntMgr.Load<Texture2D>("line_light");
            Glow = cntMgr.Load<Texture2D>("burn_line");
        }

        public void Update(GameTime gameTime, float standing)
        {
            Vector2 windowSize = _windowSizeDelegate();
            Pos1.X = windowSize.X * standing;
            Height = windowSize.Y;

            if (_starting)
            {
                _elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                _alpha += gameTime.ElapsedGameTime.Milliseconds / 750f;
                if (_elapsedTime >= 750)
                {
                    _starting = false;
                }
            } else if (_stopping)
            {
                _elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                _alpha -= gameTime.ElapsedGameTime.Milliseconds / 300f;

                if (_elapsedTime >= 300)
                {
                    _stopping = false;
                }
            } else if (_running)
            {
                _thicknessUpdateTime += gameTime.ElapsedGameTime.Milliseconds;
                _randThickness = Thickness + (float) Math.Sin(_thicknessUpdateTime * Math.PI/1000) * Thickness / 8;
                _alpha = 0.95f + (float)Math.Sin(_thicknessUpdateTime * Math.PI / 1500 + 350) * 0.05f;
                if (_thicknessUpdateTime >= 3000) _thicknessUpdateTime = 0;
            }

            _thicknessScale = _randThickness / _imageThickness;
        }

        public void DrawNormal(SpriteBatch spriteBatch)
        {
            var middleOrigin = new Vector2(0, Glow.Height / 2f);
            var middleScale = new Vector2(Height, _thicknessScale);

            spriteBatch.Draw(Glow , Pos1, null, Color.White * _alpha, MathHelper.Pi / 2, middleOrigin, middleScale, SpriteEffects.None, 0f);
        }

        public void DrawLight(SpriteBatch spriteBatch)
        {
            var middleOrigin = new Vector2(0, Light.Height / 2f);
            var middleScale = new Vector2(Height, _thicknessScale * 2);

            spriteBatch.Draw(Light, Pos1, null, Color.White * _alpha, MathHelper.Pi / 2, middleOrigin, middleScale, SpriteEffects.None, 0f);
        }

        public void Start()
        {
            _starting = true;
            _running = true;
            _elapsedTime = 0;
        }

        public void End()
        {
            _stopping = true;
            _running = false;
            _elapsedTime = 0;
        }

    }
}
