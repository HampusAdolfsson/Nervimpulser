using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArmWrestling.Components
{
    public class ParticleEngine
    {

        private readonly Random _random;
        private readonly GameMain.GetWindowSizeDelegate _windowSizeDelegate;
        private readonly GameMain.GetScaleDelegate _scaleDelegate;
        private readonly Texture2D _texture;
        private readonly List<Particle> _particles;
        private readonly Texture2D _lightMapTexture;

        private const int _particlesPerUpdate = 3;

        private int _xEmitterPos;

        public bool SpawnsParticles { get; set; }
        
        public ParticleEngine(Texture2D texture, Texture2D lightMapTexture, GameMain.GetWindowSizeDelegate windowSizeDelegate, GameMain.GetScaleDelegate scaleDelegate)
        {
            _windowSizeDelegate = windowSizeDelegate;
            _scaleDelegate = scaleDelegate;
            _texture = texture;
            _lightMapTexture = lightMapTexture;
            _particles = new List<Particle>();
            _random = new Random();
        }

        private Particle GenerateNewParticle(ref float standing, float vel, Vector2 windowSize)
        {
            Vector2 scale = _scaleDelegate();

            var position = new Vector2(_xEmitterPos, _random.Next((int) windowSize.Y));
            var velocity = new Vector2(
                    1.5f * (float)(_random.NextDouble() * 2 - 1) - vel,
                    1f * (float)(_random.NextDouble()*2 + 1)) * scale;
            var acceleration = new Vector2(0, 0.2f) * scale;
            var angle = (float) Math.Atan(velocity.Y/ velocity.X);
            var size = (float) _random.NextDouble() * 0.2f * scale.X;
            var randColor = GetColorFromStanding((float) (standing + 0.05*(_random.NextDouble() - 0.5)));
            return new Particle(_texture, _lightMapTexture, position, velocity, acceleration, angle, randColor, size, _random.Next(20) == 4);
        }

        public void Update(GameTime gameTime, float standing, int diff)
        {
            var windowSize = _windowSizeDelegate();
            _xEmitterPos = (int) (windowSize.X * standing);
            if (SpawnsParticles)
            {
                for (int i = 0; i < _particlesPerUpdate; i++)
                {
                    _particles.Add(GenerateNewParticle(ref standing, diff / 1023f * 3, windowSize));
                }
            }

            for (int p = 0; p < _particles.Count; p++)
            {
                _particles[p].Update(gameTime);
                if (_particles[p].Position.Y > windowSize.Y)
                {
                    _particles.RemoveAt(p);
                    p--;
                }
            }
        }

        public void DrawNormal(SpriteBatch spriteBatch)
        {
            foreach (var p in _particles)
            {
                p.DrawNormal(spriteBatch);
            }
        }

        public void DrawLight(SpriteBatch spriteBatch)
        {
            foreach (var p in _particles)
            {
                p.DrawLight(spriteBatch);
            }
        }

        private static Color GetColorFromStanding(float standing)
        {
            var result = new Color(0xFF, 0xEE, 0xB5);
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
