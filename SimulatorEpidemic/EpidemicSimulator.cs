using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimulatorEpidemic
{
    // Класс симуляции, реализующий интерфейс IScreen
    public class EpidemicSimulator : IScreen
    {
        private List<Human> _humans; // Список объектов человека в симуляции
        private Texture2D _humanTexture; // Текстура человека
        private Texture2D _simulationAreaTexture; // Текстура области симуляции
        private Texture2D _settingsAreaTexture; // Текстура области настроек
        private Texture2D _backgroundSimulationTexture; // Текстура фона симуляции
        private Random random; // Генератор случайных чисел

        // Поля для слайдеров
        private Slider deathChanceSlider;
        private Slider infectionRadiusSlider;
        private Slider infectionChanceSlider;
        private Slider incubationTimeSlider;
        private Slider recoveryTimeSlider;

        // Параметры симуляции
        private float InfectionChance = 0.2f;
        private float IncubationTime = 5f;
        private float RecoveryTime = 10f;
        private float DeathChance = 0.05f;
        private float DeathCheckInterval = 3f;
        private float InfectionRadius = 25f;

        private int humanCount = 50; // Общее количество людей в симуляции
        private EpidemicGraph _epidemicGraph; // График эпидемии
        private SpriteFont font_orbitiron; // Шрифт для отображения текста

        private Texture2D _sliderTexture; // Текстуры для слайдеров
        private Texture2D _knobTexture; // Текстуры для слайдеров

        private GraphicsDevice _graphicsDevice; // Графическое устройство
        private ContentManager _content; // Менеджер контента

        // Конструктор класса симулятора
        public EpidemicSimulator(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _content = content;
            _graphicsDevice = graphicsDevice;
            random = new Random();
        }

        // Инициализация
        public void Initialize()
        {
        }

        // Инициализация слайдеров
        private void InitializeSliders()
        {
            deathChanceSlider = new Slider(_sliderTexture, _knobTexture, new Vector2(1040, 50), 0.0f, 1f, DeathChance, font_orbitiron, "Death Chance");
            infectionRadiusSlider = new Slider(_sliderTexture, _knobTexture, new Vector2(1040, 150), 20f, 50f, InfectionRadius, font_orbitiron, "Infection Radius");
            infectionChanceSlider = new Slider(_sliderTexture, _knobTexture, new Vector2(1040, 250), 0.0f, 1f, InfectionChance, font_orbitiron, "Infection Chance");
            incubationTimeSlider = new Slider(_sliderTexture, _knobTexture, new Vector2(1040, 350), 0f, 15f, IncubationTime, font_orbitiron, "Incubation Time");
            recoveryTimeSlider = new Slider(_sliderTexture, _knobTexture, new Vector2(1040, 450), 0f, 15f, RecoveryTime, font_orbitiron, "Recovery Time");
        }

        // Инициализация людей
        public void InitializeHumans()
        {
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

        // Инициализация графика эпидемии
        public void InitializeGraph()
        {
            _epidemicGraph = new EpidemicGraph(_graphicsDevice, new Vector2(1010, 610), new Vector2(260, 100), humanCount);
        }

        // Метод для инициализации слайдеров и графика
        public void LoadContent()
        {
            _backgroundSimulationTexture = _content.Load<Texture2D>("backgroundSimulation"); // Загрузка текстуры фона симуляции
            _simulationAreaTexture = _content.Load<Texture2D>("SimulationArea"); // Загрузка текстуры области симуляции
            _settingsAreaTexture = _content.Load<Texture2D>("SettingsArea"); // Загрузка текстуры области настроек
            _humanTexture = _content.Load<Texture2D>("Human"); // Загрузка текстуры человека
            font_orbitiron = _content.Load<SpriteFont>("orbitiron"); // Загрузка шрифта

            _sliderTexture = _content.Load<Texture2D>("sliderTexture");
            _knobTexture = _content.Load<Texture2D>("knobTexture");

            // Инициализация после загрузки контента
            InitializeHumans();
            InitializeSliders();
            InitializeGraph();
        }

        // Обновление логики симуляции
        public void Update(GameTime gameTime)
        {
            // Подсчет количества здоровых, зараженных и выздоровевших людей
            int healthyCount = _humans.Count(h => h.State == Human.HealthState.Healthy);
            int infectedCount = _humans.Count(h => h.State == Human.HealthState.Infected);
            int recoveredCount = _humans.Count(h => h.State == Human.HealthState.Recovered);

            // Добавление новых данных в график эпидемии
            _epidemicGraph.AddDataPoints(healthyCount, infectedCount, recoveredCount);

            // Обновление состояния слайдеров и сохранение значений
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
                    Human human1 = _humans[i];
                    Human human2 = _humans[j];

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

        // Отрисовка симуляции
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Отрисовка фона
            spriteBatch.Draw(_backgroundSimulationTexture, new Rectangle(0, 0, _backgroundSimulationTexture.Width, _backgroundSimulationTexture.Height), Color.White);

            // Отрисовка области симуляции
            spriteBatch.Draw(_simulationAreaTexture, new Rectangle(10, 10, _simulationAreaTexture.Width, _simulationAreaTexture.Height), Color.White);

            // Отрисовка фона настроек
            spriteBatch.Draw(_settingsAreaTexture, new Rectangle(1010, 10, _settingsAreaTexture.Width, _settingsAreaTexture.Height), Color.White);

            // Отрисовка слайдеров
            deathChanceSlider.Draw(spriteBatch);
            infectionRadiusSlider.Draw(spriteBatch);
            infectionChanceSlider.Draw(spriteBatch);
            incubationTimeSlider.Draw(spriteBatch);
            recoveryTimeSlider.Draw(spriteBatch);

            // Отрисовка людей
            foreach (var human in _humans)
            {
                human.Draw(spriteBatch, _humanTexture);
            }

            // Отрисовка графика эпидемии
            _epidemicGraph.Draw(spriteBatch);

            // Отрисовка количества выздоровевших, здоровых и больных людей
            spriteBatch.DrawString(font_orbitiron, "Recovered: " + _humans.Count(h => h.State == Human.HealthState.Recovered), new Vector2(1020, 520), Color.Black);
            spriteBatch.DrawString(font_orbitiron, "Healthy: " + _humans.Count(h => h.State == Human.HealthState.Healthy), new Vector2(1020, 550), Color.Black);
            spriteBatch.DrawString(font_orbitiron, "Infected: " + _humans.Count(h => h.State == Human.HealthState.Infected), new Vector2(1020, 580), Color.Black);
        }
    }
}
