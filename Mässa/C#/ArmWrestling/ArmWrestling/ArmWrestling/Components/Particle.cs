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
        private const int _vel_scale = 2;

        public Texture2D Texture { get; set; }
        public Texture2D Light { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }
        public float Angle { get; set; }
        public Color Color { get; set; }
        public bool DrawsLight { get; set; }
        public Vector2 Scale;

        private readonly Rectangle _sourceRectangle;
        private readonly Vector2 _origin;
        private readonly Rectangle _sourceRectangleLight;
        private readonly Vector2 _originLight;

        public Particle(Texture2D texture, Texture2D light, Vector2 position, Vector2 velocity, Vector2 acceleration,
            float angle, Color color, float size, bool hasLight)
        {
            Texture = texture;
            Light = light;
            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;
            Angle = angle;
            Color = color;
            Scale = new Vector2(size * (velocity.Length() / _vel_scale  + 1), size);
            DrawsLight = hasLight;
            
            _sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            _origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            _sourceRectangleLight = new Rectangle(0, 0, Light.Width, Light.Height);
            _originLight = new Vector2(Light.Width / 2, Light.Height / 2);
        }

        public void Update(GameTime gameTime)
        {
            Velocity += Acceleration * gameTime.ElapsedGameTime.Milliseconds / 16;
            Position += Velocity * gameTime.ElapsedGameTime.Milliseconds / 16;
            Angle = (float) Math.Atan(Velocity.Y / Velocity.X);
            Scale.X = Velocity.Length() / _vel_scale;
        }

        public void DrawNormal(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, _sourceRectangle, Color,
                Angle, _origin, Scale, SpriteEffects.None, 0f);
        }

        public void DrawLight(SpriteBatch spriteBatch)
        {
            if (!DrawsLight) return;

            spriteBatch.Draw(Light, Position, _sourceRectangleLight, Color.White * 0.3f,
                Angle, _originLight, 12, SpriteEffects.None, 0f);
        }
    }

}
