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
        private Texture2D _backgroundMenuTexture; // Текстура фона для главного меню
        private Texture2D _backgroundSimulationTexture; // Текстура фона для симуляции
        private Texture2D _simulationAreaTexture; // Текстура для симуляции
        private Texture2D _settingsAreaTexture; // Текстура для настроек
        private Random random;

        private const float InfectionChance = 0.2f; // Вероятность заражения при столкновении
        private const float IncubationTime = 5f; // Время инкубационного периода в секундах
        private const float RecoveryTime = 10f; // Время выздоровления в секундах
        private const float DeathChance = 0.05f; // Вероятность смерти во время болезни
        private const float DeathCheckInterval = 3f; // Интервал проверки на смерть в секундах
        private const float InfectionRadius = 25f; // Радиус заражения

        // Перечисление состояний игры
        private enum GameState
        {
            MainMenu,   // Главное меню
            Simulation  // Симуляция
        }

        private GameState _currentState;    // Переменная для хранения текущего состояния игры
        private MainMenu _mainMenu;        // Объект главного меню

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
            _humanTexture = Content.Load<Texture2D>("Human");               // Загрузка текстуры человека
            _backgroundMenuTexture = Content.Load<Texture2D>("backgroundMenu"); // Загрузка текстуры фона Меню
            _backgroundSimulationTexture = Content.Load<Texture2D>("backgroundSimulation"); // Загрузка текстуры фона Симуляции
            _simulationAreaTexture = Content.Load<Texture2D>("SimulationArea");     // Загрузка текстуры области симуляции
            _settingsAreaTexture = Content.Load<Texture2D>("SettingsArea");     // Загрузка текстуры области настроек

            // Инициализация главного меню
            _mainMenu = new MainMenu(_backgroundMenuTexture);

            // Инициализация списка людей
            _humans = new List<Human>();
            for (int i = 0; i < 50; i++)
            {
                var human = new Human(_simulationAreaTexture.Width + 10, _simulationAreaTexture.Height + 10, _humanTexture.Width / 2, InfectionChance, RecoveryTime, DeathChance, DeathCheckInterval, IncubationTime, InfectionRadius);
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

            // Обновление в зависимости от текущего состояния игры
            switch (_currentState)
            {
                case GameState.MainMenu:
                    // Обновление логики главного меню
                    // Если метод Update главного меню возвращает true, переключаем состояние на симуляцию
                    if (_mainMenu.Update(gameTime))
                    {
                        _currentState = GameState.Simulation;
                    }
                    break;
                case GameState.Simulation:
                    // Обновление логики симуляции
                    UpdateSimulation(gameTime);
                    break;
            }

            base.Update(gameTime); // Вызов базового метода Update
        }

        // Метод для обновления логики симуляции
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

            // Отрисовка в зависимости от текущего состояния игры
            switch (_currentState)
            {
                case GameState.MainMenu:
                    // Отрисовка главного меню
                    _mainMenu.Draw(_spriteBatch, GraphicsDevice);
                    break;
                case GameState.Simulation:
                    // Отрисовка симуляции
                    DrawSimulation(_spriteBatch, GraphicsDevice);
                    break;
            }

            // Завершение пакетной отрисовки
            _spriteBatch.End();

            // Вызов базового метода Draw
            base.Draw(gameTime);
        }

        // В методе DrawSimulation:
        private void DrawSimulation(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {

            // Отрисовка фона
            spriteBatch.Draw(_backgroundSimulationTexture, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), Color.White);

            // Отрисовка фона симуляции
            spriteBatch.Draw(_simulationAreaTexture, new Rectangle(10, 10, _simulationAreaTexture.Width, _simulationAreaTexture.Height), Color.White);

            // Отрисовка фона настроек
            spriteBatch.Draw(_settingsAreaTexture, new Rectangle(1010, 10, _settingsAreaTexture.Width, _settingsAreaTexture.Height), Color.White);

            // Отрисовка людей в основной области
            foreach (var human in _humans)
            {
                human.Draw(_spriteBatch, _humanTexture);
            }
        }
    }
}
