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
        private Texture2D _humanTexture; // Текстура круга для отображения человека
        private Texture2D _backgroundTexture; // Текстура фона для главного меню
        private Random random;

        private const float InfectionChance = 0.2f; // Вероятность заражения при столкновении
        private const float IncubationTime = 5f; // Время инкубационного периода в секундах
        private const float RecoveryTime = 10f; // Время выздоровления в секундах
        private const float DeathChance = 0.05f; // Вероятность смерти во время болезни
        private const float DeathCheckInterval = 3f; // Интервал проверки на смерть в секундах
        private const float InfectionRadius = 25f; // Радиус заражения

        private enum GameState
        {
            MainMenu,
            Simulation
        }

        private GameState _currentState;
        private MainMenu _mainMenu;

        // Конструктор игры
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;  // Устанавливаем ширину окна
            _graphics.PreferredBackBufferHeight = 720; // Устанавливаем высоту окна
            Content.RootDirectory = "Content"; // Директория для контента
            IsMouseVisible = true; // Делает курсор мыши видимым
            random = new Random();
            _currentState = GameState.MainMenu; // Начальное состояние - главное меню
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

            // Загрузка текстуры фона
            _backgroundTexture = Content.Load<Texture2D>("backgroundMenu");

            // Инициализация главного меню
            _mainMenu = new MainMenu(_backgroundTexture);

            // Инициализация списка людей
            _humans = new List<Human>();
            for (int i = 0; i < 50; i++)
            {
                var human = new Human(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, _humanTexture.Width / 2, InfectionChance, RecoveryTime, DeathChance, DeathCheckInterval, IncubationTime, InfectionRadius);
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

            switch (_currentState)
            {
                case GameState.MainMenu:
                    if (_mainMenu.Update(gameTime))
                    {
                        _currentState = GameState.Simulation;
                    }
                    break;
                case GameState.Simulation:
                    UpdateSimulation(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        private void UpdateSimulation(GameTime gameTime)
        {
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

                    // Попытка заражения
                    human1.TryInfect(human2);

                    // Проверяем, столкнулись ли эти два человека
                    if (human1.CheckCollision(human2))
                    {
                        // Если столкновение произошло, обрабатываем его
                        human1.HandleCollision(human2);
                    }
                }
            }
        }

        // Отрисовка игры
        protected override void Draw(GameTime gameTime)
        {
            // Очистка экрана цветом "CornflowerBlue"
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Начало пакетной отрисовки
            _spriteBatch.Begin();
            switch (_currentState)
            {
                case GameState.MainMenu:
                    _mainMenu.Draw(_spriteBatch, GraphicsDevice);
                    break;
                case GameState.Simulation:
                    DrawSimulation();
                    break;
            }
            // Завершение пакетной отрисовки
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawSimulation()
        {
            // Отрисовка человека с использованием текстуры
            foreach (var human in _humans)
            {
                human.Draw(_spriteBatch, _humanTexture);
            }
        }
    }
}
