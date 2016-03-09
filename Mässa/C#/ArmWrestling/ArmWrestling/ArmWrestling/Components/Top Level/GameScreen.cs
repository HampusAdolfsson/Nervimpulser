using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ArmWrestling.Components.TopLevel
{
    internal class GameScreen
    {
        public event EventHandler PlayerWon;

        private readonly DataHandler _dataHandler;
        private ParticleEngine _particleEngine;
        private readonly Line _line; 

        private Texture2D _left, _right;
        private Rectangle _leftSourceRect, _rightSourceRect;
        private readonly GameMain.GetWindowSizeDelegate _windowSizeDelegate;
        private readonly GameMain.GetScaleDelegate _scaleDelegate;

        public bool IsRunning { get; private set; }

        private Vector2 _posLeft, _posRight;
        private float _scaleLeft, _scaleRight;

        public GameScreen(GameMain.GetWindowSizeDelegate windowSizeDelegate, GameMain.GetScaleDelegate scaleDelegate)
        {
            _windowSizeDelegate = windowSizeDelegate;
            _scaleDelegate = scaleDelegate;
            _dataHandler = new DataHandler();
            _line = new Line(50, windowSizeDelegate);
        }

        public void LoadContent(ContentManager content)
        {
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
            _dataHandler.Reset();
        }

        public void Reset()
        {
            IsRunning = false;
            _particleEngine.SpawnsParticles = false;
            _dataHandler.Reset();
        }

        public void Update(GameTime gameTime)
        {
            if (IsRunning)
            {
                _dataHandler.Update(gameTime);
                if (_dataHandler.Standing >= 1 || _dataHandler.Standing <= 0)
                {
                    IsRunning = false;
                    _particleEngine.SpawnsParticles = false;
                    _line.End();
                    PlayerWon?.Invoke(this, new PlayerWonEventArgs(_dataHandler.Standing >= 1));
                }
            }
            _particleEngine.Update(_dataHandler.Standing, _dataHandler.GetDiff());
            _line.Update(gameTime, _dataHandler.Standing);

            Vector2 window = _windowSizeDelegate();
            _leftSourceRect = new Rectangle(0, 0, (int)Math.Min(_left.Width, _left.Height * window.X / window.Y), (int)Math.Min(_left.Height, _left.Width * window.Y / window.X));
            _scaleLeft = Math.Min(window.X / _leftSourceRect.Width, window.Y / _leftSourceRect.Height);

            _rightSourceRect = new Rectangle(0, 0, (int)Math.Min(_right.Width, _right.Height * window.X / window.Y), (int)Math.Min(_right.Height, _right.Width * window.Y / window.X));
            _scaleRight = Math.Min(window.X / _rightSourceRect.Width, window.Y / _rightSourceRect.Height);

            _posLeft = new Vector2(window.X * (_dataHandler.Standing-1), 0) + _dataHandler.LeftDisplacement;
            _posRight = new Vector2(window.X * _dataHandler.Standing, 0) + _dataHandler.RightDisplacement;
        }

        public void DrawNormal(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_left, _posLeft, _leftSourceRect, Color.White, 0, Vector2.Zero, _scaleLeft, SpriteEffects.None, 0);
            spriteBatch.Draw(_right, _posRight, _rightSourceRect, Color.White, 0, Vector2.Zero, _scaleRight, SpriteEffects.None, 0);

            _particleEngine.DrawNormal(spriteBatch);
            _line.DrawNormal(spriteBatch);
        }

        public void DrawLight(SpriteBatch spriteBatch)
        {
            _particleEngine.DrawLight(spriteBatch);
            //line.DrawLight(spriteBatch);
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
