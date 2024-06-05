using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SimulatorEpidemic
{
    public class Human
    {
        public enum HealthState
        {
            Healthy,
            Infected,
            Recovered
        }

        private Vector2 _position;
        public HealthState State { get; private set; }

        public Human(int screenWidth, int screenHeight)
        {
            Random random = new Random();
            _position = new Vector2(random.Next(0, screenWidth - 20), random.Next(0, screenHeight - 20));
            State = HealthState.Healthy; // Начальное состояние - здоров
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            Color color;
            switch (State)
            {
                case HealthState.Healthy:
                    color = Color.Green;
                    break;
                case HealthState.Infected:
                    color = Color.Red;
                    break;
                case HealthState.Recovered:
                    color = Color.Blue;
                    break;
                default:
                    color = Color.White;
                    break;
            }
            spriteBatch.Draw(texture, _position, color);
        }
    }
}
