using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace ArmWrestling.Components.TopLevel
{
    class TextHandler
    {
        private int passed_time;
        private int i = 3;
        public bool Counting { get; private set; }
        public bool Waiting { get; private set; }
        public bool Victory { get; private set; }
        private string victory_text;

        public event EventHandler CountdownFinished;
        public EventHandler VictoryScreenFinished;


        private SpriteFont font;
        private GameMain.GetWindowSizeDelegate windowSizeDelegate;

        private Random random;
        private Vector2 displacement;
        private float alpha;
        private float angle;

        public TextHandler(SpriteFont font, GameMain.GetWindowSizeDelegate windowSizeDelegate)
        {
            this.font = font;
            this.windowSizeDelegate = windowSizeDelegate;
            random = new Random();
            Waiting = true;
            Counting = false;
        }

        public void StartCountdown()
        {
            Counting = true;
            Waiting = false;
            Victory = false;
            passed_time = 0;
            i = 3;
        }

        public void StartWait()
        {
            Counting = false;
            Waiting = true;
            Victory = false;
            passed_time = 0;
        }
        public void ShowVictoryScreen(bool leftWon)
        {
            Counting = false;
            Waiting = false;
            Victory = true;
            passed_time = 0;
            victory_text = leftWon ? "Blue Won!" : "Red Won!";
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && Waiting)
                StartCountdown();
            
            passed_time += gameTime.ElapsedGameTime.Milliseconds;
            if (Counting)
            {
                displacement = 5 * randInsideUnitCircle();
                angle = randomAngle();
                if (passed_time > 1000)
                {
                    i--;
                    passed_time -= 1000;
                    if (i <= 0)
                    {
                        Counting = false;
                        CountdownFinished(this, new EventArgs());
                    }
                }
            } else if (Waiting)
            {
                alpha = (float) (Math.Sin(passed_time * Math.PI / 1000) + 1) / 2;
            } else if (Victory)
            {
                if (passed_time > 3000)
                {
                    StartWait();
                    VictoryScreenFinished(this, new EventArgs());
                }
            }
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 windowSize = windowSizeDelegate();
            if (Counting)
            {
                Vector2 textSize = font.MeasureString(""+i);
                spriteBatch.DrawString(font, "" + i, new Vector2((windowSize.X) / 2, (windowSize.Y) / 2) + displacement, Color.White, angle, textSize / 2, 1f, SpriteEffects.None, 0);

            }
            else if (Waiting)
            {
                Vector2 textSize = font.MeasureString("Press space to start");
                spriteBatch.DrawString(font, "Press space to start", new Vector2(windowSize.X / 2 - textSize.X / 2, windowSize.Y / 2 - textSize.Y / 2), Color.White * alpha);
            } else if (Victory)
            {
                Vector2 textSize = font.MeasureString(victory_text);
                spriteBatch.DrawString(font, victory_text, new Vector2(windowSize.X / 2 - textSize.X / 2, windowSize.Y / 2 - textSize.Y / 2), Color.White);
            }
        }

        private Vector2 randInsideUnitCircle()
        {
            float rads = (float) (2 * Math.PI * random.NextDouble());
            float dist = (float) random.NextDouble();
            return new Vector2((float)Math.Cos(rads) * dist, (float)Math.Sin(dist) * dist);
            
        }

      
        private float randomAngle()
        {
            return (float)(random.NextDouble() * 0.1745 * 2 - 0.1745);
        }
    }
}
