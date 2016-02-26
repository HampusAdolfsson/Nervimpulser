using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArmWrestling.Components
{
    public class Particle
    {
        private const int vel_scale = 2;

        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }
        public float Angle { get; set; }
        public Color Color { get; set; }
        private Vector2 scale;

        public Particle(Texture2D texture, Vector2 position, Vector2 velocity, Vector2 acceleration,
            float angle, Color color, float size)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;
            Angle = angle;
            Color = color;
            scale = new Vector2(size * (velocity.Length() / vel_scale  + 1), size);
        }

        public void Update()
        {
            Velocity += Acceleration;
            Position += Velocity;
            Angle = (float) Math.Atan(Velocity.Y / Velocity.X);
            scale.X = Velocity.Length() / vel_scale;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            spriteBatch.Draw(Texture, Position, sourceRectangle, Color,
                Angle, origin, scale, SpriteEffects.None, 0f);
        }
    }

}
