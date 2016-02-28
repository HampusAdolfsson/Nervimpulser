using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArmWrestling.Components
{
    public class ParticleEngine
    {

        private Random random;
        private GameMain.GetWindowSizeDelegate windowSizeDelegate;
        private List<Texture2D> textures;
        private List<Particle> particles;
        private Color color;

        private int xEmitterPos;

        public bool SpawnsParticles { get; set; }

        public ParticleEngine(List<Texture2D> textures, GameMain.GetWindowSizeDelegate windowSizeDelegate)
        {
            this.windowSizeDelegate = windowSizeDelegate;
            this.textures = textures;
            particles = new List<Particle>();
            random = new Random();
            color = new Color(0xFF, 0xEE, 0xB5);
        }

        private Particle GenerateNewParticle(ref float standing, float vel, Vector2 windowSize)
        {
            var texture = textures[random.Next(textures.Count)];
            Vector2 position = new Vector2(xEmitterPos, random.Next((int) windowSize.Y));
            Vector2 velocity = new Vector2(
                    1f * (float)(random.NextDouble() * 2 - 1) + vel,
                    1f * (float)(random.NextDouble()*2 + 1));
            Vector2 acceleration = new Vector2(0, 0.1f);
            float angle = (float) Math.Atan(velocity.Y/ velocity.X);
            float size = (float) random.NextDouble() * 0.4f;
            var randColor = getColorFromStanding((float) (standing + 0.05*(random.NextDouble() - 0.5)));

            return new Particle(texture, position, velocity, acceleration, angle, randColor, size);
        }

        public void Update(float standing, int diff)
        {
            Vector2 windowSize = windowSizeDelegate();
            xEmitterPos = (int) (windowSize.X * standing);
            if (SpawnsParticles)
            {
                int toAdd = 3;
                for (int i = 0; i < toAdd; i++)
                {
                    particles.Add(GenerateNewParticle(ref standing, diff / 1023f * 3, windowSize));
                }
            }

            for (int p = 0; p < particles.Count; p++)
            {
                particles[p].Update();
                if (particles[p].Position.Y > windowSize.Y)
                {
                    particles.RemoveAt(p);
                    p--;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int p = 0; p < particles.Count; p++)
            {
                particles[p].Draw(spriteBatch);
            }
        }

        private Color getColorFromStanding(float standing)
        {
            Color result = new Color(0xFF, 0xEE, 0xB5);
            if (standing < 0.5f) result.G = (byte) (116 * Math.Pow(standing, 2) + 209);
            else
            {
                result.R = (byte)(64 * Math.Pow(standing, 2) + -154 * standing + 316);
                result.G = (byte)(-40 * Math.Pow(standing, 2) + 94 * standing + 201);
                result.B = (byte)(-74 * Math.Pow(standing, 2) + 193 * standing + 103);

            }
            return result;
        }
    }
}
