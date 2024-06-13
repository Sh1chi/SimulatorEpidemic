using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

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

        // Поля для слайдеров
        private Slider deathChanceSlider; // Слайдер для настройки вероятности смерти
        private Slider infectionRadiusSlider; // Слайдер для настройки радиуса заражения
        private Slider infectionChanceSlider; // Слайдер для настройки вероятности заражения
        private Slider incubationTimeSlider; // Слайдер для настройки времени инкубации
        private Slider recoveryTimeSlider; // Слайдер для настройки времени выздоровления

        // Поля для текстур слайдера и ручки
        private Texture2D sliderTexture; // Текстура для слайдера
        private Texture2D knobTexture; // Текстура для ручки слайдера

        // Поле для шрифта
        private SpriteFont font_orbitiron; // Шрифт для отображения текста

        private float InfectionChance = 0.2f; // Вероятность заражения при столкновении
        private float IncubationTime = 5f; // Время инкубационного периода в секундах
        private float RecoveryTime = 10f; // Время выздоровления в секундах
        private float DeathChance = 0.05f; // Вероятность смерти во время болезни
        private float DeathCheckInterval = 3f; // Интервал проверки на смерть в секундах
        private float InfectionRadius = 25f; // Радиус заражения

        private int humanCount = 50; // Общее количество людей

        private EpidemicGraph _epidemicGraph;

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

            // Загрузка текстур слайдера и ручки слайдера
            sliderTexture = Content.Load<Texture2D>("sliderTexture");
            knobTexture = Content.Load<Texture2D>("knobTexture");

            // Загрузка шрифта для отображения текста
            font_orbitiron = Content.Load<SpriteFont>("orbitiron");

            // Инициализация слайдера для вероятности смерти
            // Параметры: текстура слайдера, текстура ручки, позиция на экране, минимальное значение, максимальное значение, начальное значение, шрифт, подпись
            deathChanceSlider = new Slider(sliderTexture, knobTexture, new Vector2(1040, 50), 0.0f, 1f, DeathChance, font_orbitiron, "Death Chance");

            // Инициализация слайдера для радиуса заражения
            infectionRadiusSlider = new Slider(sliderTexture, knobTexture, new Vector2(1040, 150), 20f, 50f, InfectionRadius, font_orbitiron, "Infection Radius");

            // Инициализация слайдера для вероятности заражения
            infectionChanceSlider = new Slider(sliderTexture, knobTexture, new Vector2(1040, 250), 0.0f, 1f, InfectionChance, font_orbitiron, "Infection Chance");

            // Инициализация слайдера для времени инкубации
            incubationTimeSlider = new Slider(sliderTexture, knobTexture, new Vector2(1040, 350), 0f, 15f, IncubationTime, font_orbitiron, "Incubation Time");

            // Инициализация слайдера для времени выздоровления
            recoveryTimeSlider = new Slider(sliderTexture, knobTexture, new Vector2(1040, 450), 0f, 15f, RecoveryTime, font_orbitiron, "Recovery Time");

            // Инициализация графика
            _epidemicGraph = new EpidemicGraph(GraphicsDevice, new Vector2(1010, 610), new Vector2(260, 100), humanCount);



            // Инициализация главного меню
            _mainMenu = new MainMenu(_backgroundMenuTexture);

            // Инициализация списка людей
            _humans = new List<Human>();
            for (int i = 0; i < humanCount; i++)
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

            // Подсчет количества здоровых, зараженных и выздоровевших людей
            int healthyCount = _humans.Count(h => h.State == Human.HealthState.Healthy);  // Подсчитываем количество здоровых людей
            int infectedCount = _humans.Count(h => h.State == Human.HealthState.Infected);  // Подсчитываем количество зараженных людей
            int recoveredCount = _humans.Count(h => h.State == Human.HealthState.Recovered);  // Подсчитываем количество выздоровевших людей

            // Добавление новых данных в график эпидемии
            _epidemicGraph.AddDataPoints(healthyCount, infectedCount, recoveredCount);  // Передаем количество здоровых, зараженных и выздоровевших людей в график


            // Обновление состояния слайдеров и сохранение значения
            deathChanceSlider.Update(gameTime);
            DeathChance = deathChanceSlider.Value;

            infectionRadiusSlider.Update(gameTime);
            InfectionRadius = infectionRadiusSlider.Value;

            infectionChanceSlider.Update(gameTime);
            InfectionChance = infectionChanceSlider.Value;

            incubationTimeSlider.Update(gameTime);
            IncubationTime = incubationTimeSlider.Value;

            recoveryTimeSlider.Update(gameTime);
            RecoveryTime = recoveryTimeSlider.Value;

            // Обновление состояния каждого человека
            foreach (var human in _humans)
            {
                human.Update(gameTime);
                human.deathChance = DeathChance;
                human.infectionRadius = InfectionRadius;
                human.infectionChance = InfectionChance;
                human.incubationTime = IncubationTime;
                human.recoveryTime = RecoveryTime;
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

            // Отрисовка слайдеров
            deathChanceSlider.Draw(spriteBatch);
            deathChanceSlider.Draw(spriteBatch);
            infectionRadiusSlider.Draw(spriteBatch);
            infectionChanceSlider.Draw(spriteBatch);
            incubationTimeSlider.Draw(spriteBatch);
            recoveryTimeSlider.Draw(spriteBatch);

            // Отрисовка людей в основной области
            foreach (var human in _humans)
            {
                human.Draw(_spriteBatch, _humanTexture);
            }

            // Отрисовка графика
            _epidemicGraph.Draw(_spriteBatch);


            // Отрисовка количества выздоровевших, здоровых и больных людей
            spriteBatch.DrawString(font_orbitiron, "Recovered: " + _humans.Count(h => h.State == Human.HealthState.Recovered), new Vector2(1020, 520), Color.Black);
            spriteBatch.DrawString(font_orbitiron, "Healthy: " + _humans.Count(h => h.State == Human.HealthState.Healthy), new Vector2(1020, 550), Color.Black);
            spriteBatch.DrawString(font_orbitiron, "Infected: " + _humans.Count(h => h.State == Human.HealthState.Infected), new Vector2(1020, 580), Color.Black);


        }
    }
}
