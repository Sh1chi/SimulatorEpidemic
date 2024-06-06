using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace SimulatorEpidemic
{
    // Основной класс игры, наследующийся от Game
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics; // Управляет графическими устройствами
        private SpriteBatch _spriteBatch; // Отвечает за пакетную отрисовку спрайтов
        private List<Human> _humans; // Список объектов человека в игре
        private Texture2D _humanTexture; // Текстура для отображения человека
        private Random random; // Объект для генерации случайных чисел

        private const float InfectionChance = 0.2f; // Вероятность заражения при столкновении
        private const float RecoveryTime = 10f; // Время выздоровления в секундах
        private const float DeathChance = 0.05f; // Вероятность смерти во время болезни
        private const float DeathCheckInterval = 3f; // Интервал проверки шанса смерти в секундах


        // Конструктор игры
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content"; // Директория для контента
            IsMouseVisible = true; // Делает курсор мыши видимым
            random = new Random();
        }

        // Инициализация игры
        protected override void Initialize()
        {
            base.Initialize();
        }

        // Загрузка контента игры
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Загрузка текстуры человека
            _humanTexture = Content.Load<Texture2D>("Human");

            // Инициализация списка людей
            _humans = new List<Human>();
            for (int i = 0; i < 50; i++)
            {
                var human = new Human(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, _humanTexture.Width / 2, InfectionChance, RecoveryTime, DeathChance, DeathCheckInterval);
                if (i < 5) // Первоначально заражаем 5 человек
                {
                    human.State = Human.HealthState.Infected;
                }
                _humans.Add(human);
            }
        }

        // Обновление логики игры
        protected override void Update(GameTime gameTime)
        {
            // Проверка нажатия кнопки "Back" на геймпаде или клавиши "Escape" на клавиатуре для выхода из игры
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Обновление состояния каждого человека
            foreach (var human in _humans)
            {
                human.Update(gameTime);
            }

            // Проверка и обработка столкновений между людьми
            for (int i = 0; i < _humans.Count; i++)
            {
                for (int j = i + 1; j < _humans.Count; j++)
                {
                    Human human1 = _humans[i]; // Получаем первого человека из списка
                    Human human2 = _humans[j]; // Получаем второго человека из списка

                    // Проверяем, столкнулись ли эти два человека
                    if (human1.CheckCollision(human2))
                    {
                        // Если столкновение произошло, обрабатываем его
                        human1.HandleCollision(human2);

                        // Попытка заражения при столкновении
                        human1.TryInfect(human2);
                    }
                }
            }

                base.Update(gameTime);
        }

        // Отрисовка игры
        protected override void Draw(GameTime gameTime)
        {
            // Очистка экрана цветом "CornflowerBlue"
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Начало пакетной отрисовки
            _spriteBatch.Begin();
            // Отрисовка человека с использованием текстуры
            foreach (var human in _humans)
            {
                human.Draw(_spriteBatch, _humanTexture);
            }
            // Завершение пакетной отрисовки
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
