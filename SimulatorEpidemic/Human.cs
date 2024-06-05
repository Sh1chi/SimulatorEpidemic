using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SimulatorEpidemic
{
    public class Human
    {
        // Перечисление состояний здоровья человека
        public enum HealthState
        {
            Healthy,   // Здоров
            Infected,  // Инфицирован
            Recovered  // Выздоровевший
        }

        private Vector2 _position;       // Позиция человека на экране
        private Vector2 _direction;      // Направление движения человека
        private float _speed;            // Скорость движения человека
        private int _screenWidth;        // Ширина экрана
        private int _screenHeight;       // Высота экрана
        private float _radius;           // Радиус круга, представляющего человека
        private Random random;           // Объект для генерации случайных чисел

        public HealthState State { get; private set; }  // Текущее состояние здоровья человека

        private float changeDirectionTimer;   // Таймер для изменения направления движения
        private float timeUntilNextChange;    // Время до следующего изменения направления

        // Конструктор человека, инициализирующий его позицию, состояние здоровья и параметры движения
        public Human(int screenWidth, int screenHeight, float radius)
        {
            random = new Random();
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _radius = radius;
            _position = new Vector2(random.Next((int)_radius, screenWidth - (int)_radius), random.Next((int)_radius, screenHeight - (int)_radius));
            _speed = 100f;
            _direction = GetRandomDirection();

            // Инициализация таймера для изменения направления движения
            ResetChangeDirectionTimer();

            State = HealthState.Healthy; // Начальное состояние - здоров
        }

        // Метод для обновления состояния человека
        public void Update(GameTime gameTime)
        {
            // Обновление позиции человека
            _position += _direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Проверка столкновения с границами экрана и изменение направления при необходимости
            if (_position.X - _radius <= 0 || _position.X + _radius >= _screenWidth)
            {
                _direction.X = -_direction.X;
                _position.X = MathHelper.Clamp(_position.X, _radius, _screenWidth - _radius);
            }

            if (_position.Y - _radius <= 0 || _position.Y + _radius >= _screenHeight)
            {
                _direction.Y = -_direction.Y;
                _position.Y = MathHelper.Clamp(_position.Y, _radius, _screenHeight - _radius);
            }

            // Обновление таймера для изменения направления движения
            changeDirectionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (changeDirectionTimer >= timeUntilNextChange)
            {
                _direction = GetRandomDirection();
                ResetChangeDirectionTimer();
            }
        }

        // Метод для отрисовки человека
        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            Color color;
            // Выбор цвета в зависимости от состояния здоровья
            switch (State)
            {
                case HealthState.Healthy:
                    color = Color.Green; // Здоровый - зеленый
                    break;
                case HealthState.Infected:
                    color = Color.Red; // Инфицированный - красный
                    break;
                case HealthState.Recovered:
                    color = Color.Blue; // Выздоровевший - синий
                    break;
                default:
                    color = Color.White; // Неизвестное состояние - белый
                    break;
            }

            // Отрисовка текстуры с соответствующим цветом на текущей позиции
            spriteBatch.Draw(
                texture,
                _position,
                null,
                color,
                0f,
                new Vector2(texture.Width / 2, texture.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
        }

        // Метод для получения случайного направления движения
        private Vector2 GetRandomDirection()
        {
            // Создание случайного вектора направления
            float angle = (float)(random.NextDouble() * Math.PI * 2);
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        // Метод для сброса таймера и установки нового случайного интервала для изменения направления
        private void ResetChangeDirectionTimer()
        {
            changeDirectionTimer = 0f;
            timeUntilNextChange = (float)(random.NextDouble() * 3) + 1; // Случайный интервал от 1 до 4 секунд
        }
    }
}
