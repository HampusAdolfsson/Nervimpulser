using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace ArmWrestling.Components.TopLevel
{
    class TextHandler
    {
        private int _passedTime;
        private int _i = 3;
        public bool Counting { get; private set; }
        public bool Waiting { get; private set; }
        public bool Victory { get; private set; }
        private string _victoryText;

        public event EventHandler CountdownFinished;
        public EventHandler VictoryScreenFinished;

        private SpriteFont _genericFont;
        private SpriteFont _numbersFont;
        private readonly GameMain.GetWindowSizeDelegate _windowSizeDelegate;

        private readonly Random _random;
        private Vector2 _displacement;
        private float _alpha;
        private float _angle;

        public TextHandler(GameMain.GetWindowSizeDelegate windowSizeDelegate)
        {
            _windowSizeDelegate = windowSizeDelegate;
            _random = new Random();
            Waiting = true;
            Counting = false;
        }

        public void LoadContent(ContentManager content)
        {
            _genericFont = content.Load<SpriteFont>("announcements");
            _numbersFont = content.Load<SpriteFont>("countdown");
        }

        public void StartCountdown()
        {
            Counting = true;
            Waiting = false;
            Victory = false;
            _passedTime = 0;
            _i = 3;
        }

        public void StartWait()
        {
            Counting = false;
            Waiting = true;
            Victory = false;
            _passedTime = 0;
        }
        public void ShowVictoryScreen(bool leftWon)
        {
            Counting = false;
            Waiting = false;
            Victory = true;
            _passedTime = 0;
            _victoryText = leftWon ? "Blue Won!" : "Red Won!";
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && Waiting)
                StartCountdown();
            
            _passedTime += gameTime.ElapsedGameTime.Milliseconds;
            if (Counting)
            {
                _displacement = 5 * RandInsideUnitCircle();
                _angle = RandomAngle();
                if (_passedTime > 1000)
                {
                    _i--;
                    _passedTime -= 1000;
                    if (_i <= 0)
                    {
                        Counting = false;
                        CountdownFinished?.Invoke(this, new EventArgs());
                    }
                }
            } else if (Waiting)
            {
                _alpha = (float) (Math.Sin(_passedTime * Math.PI / 1000) + 1) / 2;
            } else if (Victory)
            {
                if (_passedTime > 3000)
                {
                    StartWait();
                    VictoryScreenFinished(this, new EventArgs());
                }
            }
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 windowSize = _windowSizeDelegate();
            if (Counting)
            {
                Vector2 textSize = _numbersFont.MeasureString(""+_i);
                spriteBatch.DrawString(_numbersFont, "" + _i, new Vector2((windowSize.X) / 2, (windowSize.Y) / 2) + _displacement, Color.White, _angle, textSize / 2, 1f, SpriteEffects.None, 0);

            }
            else if (Waiting)
            {
                Vector2 textSize = _genericFont.MeasureString("Press space to start");
                spriteBatch.DrawString(_genericFont, "Press space to start", new Vector2(windowSize.X / 2 - textSize.X / 2, windowSize.Y / 2 - textSize.Y / 2), Color.White * _alpha);
            } else if (Victory)
            {
                Vector2 textSize = _genericFont.MeasureString(_victoryText);
                spriteBatch.DrawString(_genericFont, _victoryText, new Vector2(windowSize.X / 2 - textSize.X / 2, windowSize.Y / 2 - textSize.Y / 2), Color.White);
            }
        }

        private Vector2 RandInsideUnitCircle()
        {
            float rads = (float) (2 * Math.PI * _random.NextDouble());
            float dist = (float) _random.NextDouble();
            return new Vector2((float)Math.Cos(rads) * dist, (float)Math.Sin(dist) * dist);
            
        }

      
        private float RandomAngle()
        {
            return (float)(_random.NextDouble() * 0.1745 * 2 - 0.1745);
        }
    }
}
