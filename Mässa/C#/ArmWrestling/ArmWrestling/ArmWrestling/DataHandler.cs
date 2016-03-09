using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ArmWrestling
{
    internal class DataHandler
    {
        private readonly Random _random;

        public Vector2 LeftDisplacement { get; private set; }
        public Vector2 RightDisplacement { get; private set; }
        private int _elapsedMillis;

        private int _lastLeft;
        private int _lastRight;

        public float Standing { get; private set; }

        public DataHandler()
        {
            Standing = 0.5f;
            _random = new Random();

            Server.ReceivedData += data =>
            {
                var parts = data.Split('.');
                int left, right;
                if (!int.TryParse(parts[0], out left) || !int.TryParse(parts[1], out right)) return;
                _lastLeft = left;
                _lastRight = right;
            };
            Server.Start();
        }

        public void Update(GameTime gameTime)
        {
            
            _elapsedMillis += gameTime.ElapsedGameTime.Milliseconds;

            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Left))
            {
                _lastRight = 500;
            }
            if (state.IsKeyDown(Keys.Right))
            {
                _lastLeft = 500;
            }
            Standing -= (float)Math.Pow((_lastRight - _lastLeft), 3) / 10000000000 * 6 * gameTime.ElapsedGameTime.Milliseconds / 1000f
                + 0.5f * (_lastRight - _lastLeft) *500000 /10000000000 * 6 * gameTime.ElapsedGameTime.Milliseconds / 1000f;

            if (_elapsedMillis > 80)
            {
                _elapsedMillis -= 80;
                RightDisplacement = RandInsideUnitCircle() * _lastRight / 70f;
                LeftDisplacement = RandInsideUnitCircle() * _lastLeft / 70f;
            }
            Server.Send(Standing.ToString("R"));

        }

        public void Reset()
        {
            Standing = 0.5f;
            _lastLeft = 0;
            _lastRight = 0;
            _elapsedMillis = 0;
            RightDisplacement = LeftDisplacement = Vector2.Zero;
        }

        public int GetDiff()
        {
            return _lastRight - _lastLeft;
        }

        private Vector2 RandInsideUnitCircle()
        {
            var rads = (2 * Math.PI * _random.NextDouble());
            var dist = _random.NextDouble();
            return new Vector2((float)(Math.Cos(rads) * dist), (float)(Math.Sin(rads) * dist));
        }
    }
}
