using System;
using System.Threading;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ArmWrestling
{
    class DataHandler
    {
        private Process process;
        private Random random;

        public Vector2 LeftDisplacement { get; private set; }
        public Vector2 RightDisplacement { get; private set; }
        private int elapsed_millis;

        private int last_left;
        private int last_right;

        public float Standing { get; private set; }

        public DataHandler(Process inputProcess)
        {
            process = inputProcess;
            Standing = 0.5f;
            random = new Random();
            new Thread(readStream).Start();
        }

        public void Update(GameTime gameTime)
        {
            
            elapsed_millis += gameTime.ElapsedGameTime.Milliseconds;

            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Left))
            {
                last_right = 500;
            }
            if (state.IsKeyDown(Keys.Right))
            {
                last_left = 500;
            }
            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000f;
            Standing -= (float)Math.Pow((last_right - last_left), 3) / 10000000000 * 6 * gameTime.ElapsedGameTime.Milliseconds / 1000f
                + 0.5f * (last_right - last_left) *500000 /10000000000 * 6 * gameTime.ElapsedGameTime.Milliseconds / 1000f;

            if (elapsed_millis > 80)
            {
                elapsed_millis -= 80;
                RightDisplacement = randInsideUnitCircle() * last_right / 70f;
                LeftDisplacement = randInsideUnitCircle() * last_left / 70f;
            }

        }

        public void Reset()
        {
            Standing = 0.5f;
            last_left = 0;
            last_right = 0;
            elapsed_millis = 0;
            process.StandardOutput.BaseStream.Flush();
            RightDisplacement = LeftDisplacement = Vector2.Zero;
        }

        public int GetDiff()
        {
            return last_right - last_left;
        }

        private Vector2 randInsideUnitCircle()
        {
            var rads = (2 * Math.PI * random.NextDouble());
            var dist = random.NextDouble();
            return new Vector2((float)(Math.Cos(rads) * dist), (float)(Math.Sin(rads) * dist));
        }

        private void readStream()
        {
            while (true)
            {
                while (!process.StandardOutput.EndOfStream)
                {
                    string input = process.StandardOutput.ReadLine();
                    Console.WriteLine(input);
                    string[] parts = input.Split('.');
                    int left, right;
                    if (Int32.TryParse(parts[0], out left) && Int32.TryParse(parts[1], out right))
                    {
                        last_left = left;
                        last_right = right;
                    }
                    
                }
                Thread.Sleep(2);
            }
        }
    }
}
