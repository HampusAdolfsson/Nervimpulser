using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ArmWrestling.Components.TopLevel
{
    internal class GameScreen
    {
        public event EventHandler PlayerWon;
        
        private ParticleEngine _particleEngine;
        private readonly Line _line;

        private SpriteFont _pointsFont;
        private string _points_left = "Points: 000000", _points_right = "Points: 000000";
        private Vector2 _points_pos_left, _points_pos_right;

        private Texture2D _left, _right;
        private Rectangle _leftSourceRect, _rightSourceRect;
        private readonly GameMain.GetWindowSizeDelegate _windowSizeDelegate;
        private readonly GameMain.GetScaleDelegate _scaleDelegate;
        
        private Vector2 _posLeft, _posRight;
        private float _scaleLeft, _scaleRight;

        public bool IsRunning { get; private set; }


        public GameScreen(GameMain.GetWindowSizeDelegate windowSizeDelegate, GameMain.GetScaleDelegate scaleDelegate)
        {
            _windowSizeDelegate = windowSizeDelegate;
            _scaleDelegate = scaleDelegate;
            _line = new Line(50, windowSizeDelegate);
        }

        public void LoadContent(ContentManager content)
        {
            _pointsFont = content.Load<SpriteFont>("fps");

            _left = content.Load<Texture2D>("wrestling_sprite_blue");
            _right = content.Load<Texture2D>("wrestling_sprite_red");
            
            var particle = content.Load<Texture2D>("particle");
            var particleLight = content.Load<Texture2D>("particle_light");
            _particleEngine = new ParticleEngine(particle, particleLight, _windowSizeDelegate, _scaleDelegate);
            _line.LoadContent(content);
        }

        public void Start()
        {
            IsRunning = true;
            _particleEngine.SpawnsParticles = true;
            _line.Start();
            DataHandler.Instance.Reset();
        }

        public void Reset()
        {
            IsRunning = false;
            _particleEngine.SpawnsParticles = false;
            DataHandler.Instance.Reset();
        }

        public void Update(GameTime gameTime)
        {
            if (IsRunning)
            {
                DataHandler.Instance.Update(gameTime);
                if (DataHandler.Instance.Standing >= 1 || DataHandler.Instance.Standing <= 0)
                {
                    IsRunning = false;
                    _particleEngine.SpawnsParticles = false;
                    _line.End();
                    PlayerWon?.Invoke(this, new PlayerWonEventArgs(DataHandler.Instance.Standing >= 1));
                }
                _points_left = "Points: " + (DataHandler.Instance._lastLeft*976).ToString("D6");
                _points_right = "Points: " + (DataHandler.Instance._lastRight*976).ToString("D6");
            }
            _particleEngine.Update(gameTime, DataHandler.Instance.Standing, DataHandler.Instance.GetDiff());
            _line.Update(gameTime, DataHandler.Instance.Standing);

            Vector2 window = _windowSizeDelegate();
            _leftSourceRect = new Rectangle(0, 0, (int)Math.Min(_left.Width, _left.Height * window.X / window.Y), (int)Math.Min(_left.Height, _left.Width * window.Y / window.X));
            _scaleLeft = Math.Min(window.X / _leftSourceRect.Width, window.Y / _leftSourceRect.Height);

            _rightSourceRect = new Rectangle(0, 0, (int)Math.Min(_right.Width, _right.Height * window.X / window.Y), (int)Math.Min(_right.Height, _right.Width * window.Y / window.X));
            _scaleRight = Math.Min(window.X / _rightSourceRect.Width, window.Y / _rightSourceRect.Height);

            _posLeft = new Vector2(window.X * (DataHandler.Instance.Standing-1), 0) + DataHandler.Instance.LeftDisplacement;
            _posRight = new Vector2(window.X * DataHandler.Instance.Standing, 0) + DataHandler.Instance.RightDisplacement;

            _points_pos_left = new Vector2(20, window.Y - 30);
            _points_pos_right = new Vector2(window.X - 170, window.Y - 30);
        }

        public void DrawNormal(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_left, _posLeft, _leftSourceRect, Color.White, 0, Vector2.Zero, _scaleLeft, SpriteEffects.None, 0);
            spriteBatch.Draw(_right, _posRight, _rightSourceRect, Color.White, 0, Vector2.Zero, _scaleRight, SpriteEffects.None, 0);

            _particleEngine.DrawNormal(spriteBatch);
            _line.DrawNormal(spriteBatch);

            spriteBatch.DrawString(_pointsFont, _points_left, _points_pos_left, Color.White);
            spriteBatch.DrawString(_pointsFont, _points_right, _points_pos_right, Color.White);
        }

        public void DrawLight(SpriteBatch spriteBatch)
        {
            _particleEngine.DrawLight(spriteBatch);
            _line.DrawLight(spriteBatch);


            spriteBatch.DrawString(_pointsFont, _points_left, _points_pos_left, Color.White);
            spriteBatch.DrawString(_pointsFont, _points_right, _points_pos_right, Color.White);
        }

        public class PlayerWonEventArgs : EventArgs
        {
            public bool LeftWon { get; private set; }
            public PlayerWonEventArgs(bool leftWon)
            {
                LeftWon = leftWon;
            }
        }

    }
}
