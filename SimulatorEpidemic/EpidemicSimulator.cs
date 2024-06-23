using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

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
        private Texture2D _buttonBackTexture; // Текстура для кнопки "Назад"
        private Texture2D _buttonRetryTexture; // Текстура для кнопки "Повторить"
        private Texture2D _buttonStartTexture; // Текстура для кнопки "Начать"
        private Texture2D _buttonAreaTexture; // Текстура для области кнопок
        private Texture2D _graphAreaTexture; // Текстура для области графика
        private Texture2D _healthStatusAreaTexture; // Текстура для области статуса здоровья
        private Texture2D _nameAreaTexture; // Текстура для области имени
        private Texture2D _newsAreaTexture; // Текстура для области новостей
        private Texture2D _videoAreaTexture; // Текстура для области видео
        private Random random; // Генератор случайных чисел

        private NewsManager _newsManager; // Объект для управления новостями
        private string[] newsArray; // Массив строк для хранения новостных сообщений
        private SoundEffect typingSound; // Звуковой эффект для звука набора текста

        SoundEffect button_sound; // Звуковой эффект кнопки
        SoundEffectInstance button_sound_Instance; // Экземпляр звукового эффекта кнопки

        private GifAnimation _gifAnimation; // Объект для анимации GIF


        // Поля для слайдеров
        private Slider deathChanceSlider;
        private Slider infectionRadiusSlider;
        private Slider infectionChanceSlider;
        private Slider incubationTimeSlider;
        private Slider recoveryTimeSlider;
        private Slider speedSlider; // Ползунок для изменения скорости людей
        private Slider initialHumanCountSlider; // Ползунок для изменения начального количества людей
        private Texture2D _sliderTexture; // Текстуры для слайдеров
        private Texture2D _knobTexture; // Текстуры для слайдеров

        // Параметры симуляции
        private float InfectionChance = 0.2f;
        private float IncubationTime = 5f;
        private float RecoveryTime = 10f;
        private float DeathChance = 0.05f;
        private float DeathCheckInterval = 3f;
        private float InfectionRadius = 25f;
        private int Speed = 100;
        private int humanCount = 100; // Общее количество людей в симуляции

        private EpidemicGraph _epidemicGraph; // График эпидемии
        private SpriteFont font_orbitiron12; // Шрифт для отображения текста
        private SpriteFont font_orbitiron14; // Шрифт для отображения текста
        private SpriteFont font_orbitiron16; // Шрифт для отображения текста

        private GraphicsDevice _graphicsDevice; // Графическое устройство
        private ContentManager _content; // Менеджер контента

        // Кнопки
        private Button startButton;
        private Button backButton;
        private Button retryButton;

        private enum SimulationState
        {
            NotStarted, // Состояние: не начато
            Running,    // Состояние: выполняется
            Stopped     // Состояние: остановлено
        }
        private SimulationState _simulationState; // Текущее состояние симуляции

        private Rectangle _buttonBackRectangle; // Прямоугольник для кнопки "Назад"
        private Rectangle _buttonRetryRectangle; // Прямоугольник для кнопки "Повторить"
        private Rectangle _buttonStartRectangle; // Прямоугольник для кнопки "Старт"

        private Rectangle _initialHumanCountSliderRectangle;

        private MouseState _previousMouseState; // Предыдущее состояние мыши

        // Конструктор класса симулятора
        public EpidemicSimulator(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _content = content;
            _graphicsDevice = graphicsDevice;
            random = new Random();
            _simulationState = SimulationState.NotStarted;
        }

        // Инициализация
        public void Initialize()
        {
        }

        // Инициализация слайдеров
        private void InitializeSliders()
        {
            deathChanceSlider = new Slider(_sliderTexture, _knobTexture, new Vector2(1300, 370), 0.0f, 1f, DeathChance, font_orbitiron12, "Death Chance");
            infectionRadiusSlider = new Slider(_sliderTexture, _knobTexture, new Vector2(1300, 470), 20f, 50f, InfectionRadius, font_orbitiron12, "Infection Radius");
            infectionChanceSlider = new Slider(_sliderTexture, _knobTexture, new Vector2(1300, 570), 0.0f, 1f, InfectionChance, font_orbitiron12, "Infection Chance");
            incubationTimeSlider = new Slider(_sliderTexture, _knobTexture, new Vector2(1600, 370), 0f, 15f, IncubationTime, font_orbitiron12, "Incubation Time");
            recoveryTimeSlider = new Slider(_sliderTexture, _knobTexture, new Vector2(1600, 470), 0f, 15f, RecoveryTime, font_orbitiron12, "Recovery Time");
            speedSlider = new Slider(_sliderTexture, _knobTexture, new Vector2(1600, 570), 20f, 200f, Speed, font_orbitiron12, "Speed");
            initialHumanCountSlider = new Slider(_sliderTexture, _knobTexture, new Vector2(1450, 670), 50, 150, humanCount, font_orbitiron12, "Human Count");
        }

        // Инициализация людей
        public void InitializeHumans()
        {
            _humans = new List<Human>();
            for (int i = 0; i < humanCount; i++)
            {
                var human = new Human(_simulationAreaTexture.Width, _simulationAreaTexture.Height, _humanTexture.Width / 2, InfectionChance, RecoveryTime, DeathChance, DeathCheckInterval, IncubationTime, InfectionRadius, Speed);
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
            _epidemicGraph = new EpidemicGraph(_graphicsDevice, new Vector2(20, 914), new Vector2(1180, 146), humanCount);
        }

        // Метод для инициализации слайдеров и графика
        public void LoadContent()
        {
            // Загрузка текстур фоновых элементов и интерфейса
            _humanTexture = _content.Load<Texture2D>("Human"); // Загрузка текстуры человека
            _backgroundSimulationTexture = _content.Load<Texture2D>("backgroundSimulation"); // Загрузка текстуры фона симуляции
            _simulationAreaTexture = _content.Load<Texture2D>("SimulationArea"); // Загрузка текстуры области симуляции
            _settingsAreaTexture = _content.Load<Texture2D>("SettingArea1"); // Загрузка текстуры области настроек
            _graphAreaTexture = _content.Load<Texture2D>("GraphArea"); // Загрузка текстуры для области графика
            _healthStatusAreaTexture = _content.Load<Texture2D>("health_status_area"); // Загрузка текстуры для области статуса здоровья
            _nameAreaTexture = _content.Load<Texture2D>("NameArea"); // Загрузка текстуры для области имени
            _newsAreaTexture = _content.Load<Texture2D>("NewsArea"); // Загрузка текстуры для области новостей
            _videoAreaTexture = _content.Load<Texture2D>("VideoArea"); // Загрузка текстуры для области видео

            // Загрузка текстур кнопок и элементов интерфейса
            _buttonBackTexture = _content.Load<Texture2D>("button_BACK"); // Загрузка текстуры для кнопки "Назад"
            _buttonRetryTexture = _content.Load<Texture2D>("button_RETRY"); // Загрузка текстуры для кнопки "Повторить"
            _buttonStartTexture = _content.Load<Texture2D>("button_START_2"); // Загрузка текстуры для кнопки "Начать"
            _buttonAreaTexture = _content.Load<Texture2D>("ButtonArea"); // Загрузка текстуры для области кнопок

            // Загрузка текстур для слайдеров
            _sliderTexture = _content.Load<Texture2D>("sliderTexture"); // Загрузка текстуры для слайдера
            _knobTexture = _content.Load<Texture2D>("knobTexture"); // Загрузка текстуры для ручки слайдера

            // Загрузка звуковых эффектов и других ресурсов
            font_orbitiron12 = _content.Load<SpriteFont>("orbitiron12"); // Загрузка шрифта
            font_orbitiron14 = _content.Load<SpriteFont>("orbitiron14"); // Загрузка шрифта
            font_orbitiron16 = _content.Load<SpriteFont>("orbitiron16"); // Загрузка шрифта
            typingSound = _content.Load<SoundEffect>("texting"); // Загрузка звука набора текста
            LoadNews(); // Загрузка новостей

            // Инициализация объектов и компонентов
            _newsManager = new NewsManager(newsArray, 5, 0.05, typingSound); // Инициализация NewsManager с передачей звука
            _gifAnimation = new GifAnimation(_graphicsDevice, "C:\\Users\\Shevc\\OneDrive\\Desktop\\Practice2\\videoplayback5.gif");
            InitializeSliders(); // Инициализация слайдеров


            // Загрузка и настройка звуковых эффектов
            button_sound = _content.Load<SoundEffect>("button_sound"); // Загрузка звукового эффекта кнопки
            button_sound_Instance = button_sound.CreateInstance(); // Создание экземпляра звукового эффекта кнопки
            button_sound_Instance.Volume = 0.4f; // Установка громкости звукового эффекта

            // Инициализация кнопок
            _buttonBackTexture = _content.Load<Texture2D>("button_BACK");
            _buttonRetryTexture = _content.Load<Texture2D>("button_RETRY");
            _buttonStartTexture = _content.Load<Texture2D>("button_START_2");
            button_sound = _content.Load<SoundEffect>("button_sound");

            startButton = new Button(_buttonStartTexture, new Rectangle(1463, 1005, _buttonStartTexture.Width, _buttonStartTexture.Height), button_sound);
            backButton = new Button(_buttonBackTexture, new Rectangle(1685, 1005, _buttonBackTexture.Width, _buttonBackTexture.Height), button_sound);
            retryButton = new Button(_buttonRetryTexture, new Rectangle(1240, 1005, _buttonRetryTexture.Width, _buttonRetryTexture.Height), button_sound);
        }

        // Метод для загрузки новостей
        private void LoadNews()
        {
            newsArray = new string[]
            {
            "// \"NEW VACCINE SHOWS 90% EFFECTIVENESS IN CLINICAL TRIALS\"\n\n" +
            "Leading scientists state that this vaccine could be the key to ending the pandemic. Mass production is set to begin in the coming months.",

                "// \"VIRUS SPREADS TO FIVE NEW COUNTRIES IN THE LAST 24 HOURS\"\n\n" +
            "The World Health Organization warns of the need to strengthen safety measures. Local authorities are introducing additional restrictions.",

                "// \"SCIENTISTS DISCOVER A NEW VIRUS STRAIN WITH INCREASED TRANSMISSIBILITY\"\n\n" +
                "The new strain spreads faster and requires further research. Efforts are underway to develop updated vaccines.",

                "// \"GOVERNMENT ANNOUNCES STRICT LOCKDOWN IN THE CAPITAL\"\n\n" +
            "The restrictive measures include closing schools and public places. Residents are advised to stay home and avoid unnecessary travel.",

                "// \"INTERNATIONAL FLIGHTS SUSPENDED DUE TO GLOBAL PANDEMIC\"\n\n" +
            "Airlines are canceling thousands of flights to prevent further spread of the virus. Passengers are facing mass delays and cancellations.",

                "// \"NEW ANTIVIRAL DRUG DEVELOPED, SPEEDING UP RECOVERY\"\n\n" +
            "The drug has already shown positive results in early trials. Mass production is expected to begin soon.",

                "// \"COUNTRY'S ECONOMY SUFFERS FROM PANDEMIC IMPACTS, UNEMPLOYMENT RATE RISES\"\n\n" +
            "The government is developing support measures for affected sectors. Economists predict a slow recovery.",

                "// \"MEDICAL FACILITIES OVERWHELMED, SHORTAGE OF EQUIPMENT AND STAFF\"\n\n" +
            "Doctors are working overtime to cope with the influx of patients. Volunteers and reservists are being called in to help.",

                "// \"MASS VACCINATION CAMPAIGN LAUNCHED\"\n\n" +
            "Vaccinations are being carried out at specially organized centers and mobile units. Residents are urged to sign up for vaccinations in advance.",

                "// \"INFECTION OUTBREAK IN NEIGHBORING COUNTRY RAISES CONCERNS AMONG AUTHORITIES\"\n\n" +
            "Border controls are being tightened to prevent the virus from spreading. Medical services are on high alert."
            };
        }

        // Обновление логики симуляции
        public void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();

            startButton.Update(currentMouseState);
            backButton.Update(currentMouseState);
            retryButton.Update(currentMouseState);

            if (startButton.IsClicked)
            {
                _simulationState = SimulationState.Running;
                initialHumanCountSlider.IsEnabled = false; // Отключение ползунка "Human Count"
                startButton.IsEnabled = false; // Отключение кнопки "Start"
                InitializeHumans();
                InitializeGraph();
            }

            if (retryButton.IsClicked)
            {
                ResetSimulation();
            }

            if (backButton.IsClicked)
            {
                ResetSimulation();
                _simulationState = SimulationState.Stopped;
                GameStateManager.Instance.ChangeScreen("MainMenu");
            }

            _previousMouseState = currentMouseState;

            // Обновление состояния и значений слайдеров
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

            speedSlider.Update(gameTime);
            Speed = (int)speedSlider.Value;

            if (_simulationState != SimulationState.Running)
            {
                initialHumanCountSlider.Update(gameTime);
                humanCount = (int)initialHumanCountSlider.Value;
            }
            // Если симуляция запущена, выполняем обновление состояний
            if (_simulationState == SimulationState.Running)
            {
                // Подсчет количества здоровых, зараженных и выздоровевших людей
                int healthyCount = _humans.Count(h => h.State == Human.HealthState.Healthy);
                int infectedCount = _humans.Count(h => h.State == Human.HealthState.Infected);
                int deadCount = _humans.Count(h => h.State == Human.HealthState.Dead);

                // Обновление анимации GIF
                _gifAnimation.Update(gameTime);

                // Добавление новых данных в график эпидемии
                _epidemicGraph.AddDataPoints(healthyCount, infectedCount, deadCount);

                // Обновление состояния каждого человека
                foreach (var human in _humans)
                {
                    if (human.State != Human.HealthState.Dead)
                    {
                        human.Update(gameTime); // Обновление состояния человека
                        human.deathChance = DeathChance; // Установка шанса смерти
                        human.infectionRadius = InfectionRadius; // Установка радиуса инфекции
                        human.infectionChance = InfectionChance; // Установка шанса заражения
                        human.incubationTime = IncubationTime; // Установка времени инкубации
                        human.recoveryTime = RecoveryTime; // Установка времени выздоровления
                        human.speed = Speed; 
                    }
                }

                // Проверка и обработка столкновений между людьми
                for (int i = 0; i < _humans.Count; i++)
                {
                    for (int j = i + 1; j < _humans.Count; j++)
                    {
                        Human human1 = _humans[i];
                        Human human2 = _humans[j];

                        // Попытка заражения между двумя людьми
                        human1.TryInfect(human2);

                        // Проверка на столкновение между двумя людьми
                        if (human1.CheckCollision(human2))
                        {
                            // Если столкновение произошло, обрабатываем его
                            human1.HandleCollision(human2);
                        }
                    }
                }
            }
        }

        // Сброс состояния симуляции
        public void ResetSimulation()
        {
            _simulationState = SimulationState.NotStarted; // Сброс состояния симуляции на "Не начато"
            startButton.IsEnabled = true; // Включение кнопки "Start"
            InitializeHumans(); // Переинициализация людей
            InitializeGraph(); // Переинициализация графика
            InitializeSliders(); // Переинициализация слайдеров
            _newsManager.ResetNews(); // Сброс новостей в менеджере новостей
        }

        // Отрисовка симуляции
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_backgroundSimulationTexture, new Rectangle(0, 0, _backgroundSimulationTexture.Width, _backgroundSimulationTexture.Height), Color.White);      // Отрисовка фона
            spriteBatch.Draw(_simulationAreaTexture, new Rectangle(10, 90, _simulationAreaTexture.Width, _simulationAreaTexture.Height), Color.White);      // Отрисовка области симуляции
            spriteBatch.Draw(_settingsAreaTexture, new Rectangle(1230, 254, _settingsAreaTexture.Width, _settingsAreaTexture.Height), Color.White);     // Отрисовка фона настроек
            spriteBatch.Draw(_nameAreaTexture, new Rectangle(530, 10, _nameAreaTexture.Width, _nameAreaTexture.Height), Color.White);
            spriteBatch.Draw(_newsAreaTexture, new Rectangle(1230, 780, _newsAreaTexture.Width, _newsAreaTexture.Height), Color.White);
            spriteBatch.Draw(_graphAreaTexture, new Rectangle(10, 780, _graphAreaTexture.Width, _graphAreaTexture.Height), Color.White);
            spriteBatch.Draw(_healthStatusAreaTexture, new Rectangle(20, 790, _healthStatusAreaTexture.Width, _healthStatusAreaTexture.Height), Color.White);
            //spriteBatch.Draw(_buttonAreaTexture, new Rectangle(1230, 1000, _buttonAreaTexture.Width, _buttonAreaTexture.Height), Color.White);

            // Отрисовка кнопок
            startButton.Draw(spriteBatch);
            backButton.Draw(spriteBatch);
            retryButton.Draw(spriteBatch);

            // Отрисовка слайдеров
            deathChanceSlider.Draw(spriteBatch);
            infectionRadiusSlider.Draw(spriteBatch);
            infectionChanceSlider.Draw(spriteBatch);
            incubationTimeSlider.Draw(spriteBatch);
            recoveryTimeSlider.Draw(spriteBatch);
            speedSlider.Draw(spriteBatch);
            initialHumanCountSlider.Draw(spriteBatch);

            if (_simulationState == SimulationState.Running)
            {
                // Рисуем текущий кадр видео анимации на экране
                _gifAnimation.Draw(spriteBatch, new Vector2(1325, 90));

                // Отрисовка людей
                foreach (var human in _humans)
                {
                    human.Draw(spriteBatch, _humanTexture);
                }

                // Отрисовка графика эпидемии
                _epidemicGraph.Draw(spriteBatch);

                // Отрисовка количества выздоровевших, здоровых и больных людей
                spriteBatch.DrawString(font_orbitiron14, "// " + _humans.Count(h => h.State == Human.HealthState.Healthy), new Vector2(492, 844), Color.White);
                spriteBatch.DrawString(font_orbitiron14, "// " + _humans.Count(h => h.State == Human.HealthState.Infected), new Vector2(770, 844), Color.White);
                spriteBatch.DrawString(font_orbitiron14, "// " + _humans.Count(h => h.State == Human.HealthState.Dead), new Vector2(1056, 844), Color.White);
                spriteBatch.DrawString(font_orbitiron16, "// " + humanCount, new Vector2(300, 857), Color.White);

                // Вывод новостей
                string currentNews = _newsManager.GetCurrentNews(gameTime); // Получение текущей новости
                string wrappedNews = _newsManager.WrapText(currentNews, font_orbitiron12, _newsAreaTexture.Width - 40); // Перенос текста
                spriteBatch.DrawString(font_orbitiron12, wrappedNews, new Vector2(1250, 850), Color.White); // Отображение текста
            }
        }
    }
}
