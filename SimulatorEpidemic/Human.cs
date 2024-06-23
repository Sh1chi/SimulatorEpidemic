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
            Incubating, // Инкубационный период
            Infected,  // Инфицирован
            Recovered,  // Выздоровевший
            Dead       // Мертвый

        }

        private Vector2 _position;       // Позиция человека на экране
        private Vector2 _direction;      // Направление движения человека
        private float _radius;           // Радиус круга, представляющего человека

        private int _screenWidth;        // Ширина экрана
        private int _screenHeight;       // Высота экрана

        public float infectionRadius;   // Радиус заражения
        public float infectionChance;   // Вероятность заражения при столкновении
        public float incubationTime;    // Время инкубационного периода в секундах
        public float recoveryTime;      // Время выздоровления в секундах
        public float deathChance;      // Вероятность смерти во время болезни
        public int speed;      // Вероятность смерти во время болезни

        private float infectionTimer;    // Таймер для отслеживания времени заражения

        private float deathCheckInterval; // Интервал проверки на смерть в секунда

        private float deathCheckTimer;   // Таймер для отслеживания времени до следующей проверки на смерть
        private Random random;           // Объект для генерации случайных чисел

        private float changeDirectionTimer;   // Таймер для изменения направления движения
        private float timeUntilNextChange;    // Время до следующего изменения направления

        public HealthState State { get; set; }  // Текущее состояние здоровья человека


        // Конструктор человека, инициализирующий его позицию, состояние здоровья и параметры движения
        public Human(int screenWidth, int screenHeight, float radius, float infectionChance, float recoveryTime, float deathChance, float deathCheckInterval, float incubationTime, float infectionRadius, int speed)
        {
            random = new Random();
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _radius = radius;
            this.infectionChance = infectionChance;
            this.recoveryTime = recoveryTime;
            this.deathChance = deathChance;
            this.deathCheckInterval = deathCheckInterval;
            this.incubationTime = incubationTime;
            this.infectionRadius = infectionRadius;
            this.speed = speed;

            // Учитываем смещение области симуляции
            int offsetX = 10;
            int offsetY = 90;
            _position = new Vector2(random.Next((int)_radius + offsetX + 1, screenWidth + offsetX - (int)_radius),
                                    random.Next((int)_radius + offsetY + 1, screenHeight + offsetY - (int)_radius));
            
            _direction = GetRandomDirection();

            // Инициализация таймера для изменения направления движения
            ResetChangeDirectionTimer();

            State = HealthState.Healthy; // Начальное состояние - здоров
            infectionTimer = 0f; // Инициализация таймера заражения
            deathCheckTimer = 0f; // Инициализация таймера проверки на смерть
        }

        // Метод для обновления состояния человека
        public void Update(GameTime gameTime)
        {
            // Обновление позиции человека
            _position += _direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Проверка столкновения с границами экрана и изменение направления при необходимости
            CheckScreenCollision();

            // Обновление таймера для изменения направления движения
            UpdateDirectionTimer(gameTime);

            // Обновление таймера заражения и проверка выздоровления
            UpdateInfectionTimer(gameTime);
        }

        // Метод для отрисовки человека
        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            Color color = GetHealthColor();

            // Отрисовка зоны заражения
            if (State == HealthState.Infected || State == HealthState.Incubating)
            {
                Texture2D circle = CreateCircleTexture(spriteBatch.GraphicsDevice, (int)infectionRadius * 2);
                spriteBatch.Draw(circle, _position - new Vector2(infectionRadius, infectionRadius), Color.Red * 0.2f);
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

        // Проверка на столкновение с другим объектом
        public bool CheckCollision(Human other)
        {
            // Мертвые не участвуют в коллизиях
            if (this.State == HealthState.Dead || other.State == HealthState.Dead)
                return false;

            // Рассчитываем расстояние между центрами двух объектов
            float distance = Vector2.Distance(_position, other._position);
            // Проверяем, меньше ли это расстояние суммы радиусов двух объектов
            return distance < _radius + other._radius;
        }

        // Обработка столкновения с другим объектом
        public void HandleCollision(Human other)
        {
            // Вычисляем нормаль столкновения
            Vector2 normal = _position - other._position;
            normal.Normalize();

            // Рассчитываем относительную скорость
            Vector2 relativeVelocity = _direction - other._direction;
            // Находим скорость вдоль нормали
            float velocityAlongNormal = Vector2.Dot(relativeVelocity, normal);

            // Не обрабатываем столкновение, если скорости разделяются
            if (velocityAlongNormal > 0)
                return;

            float restitution = 1f; // Коэффициент восстановления (1 - идеально упругий удар)
            // Вычисляем импульс
            float impulseScalar = -(1 + restitution) * velocityAlongNormal;
            impulseScalar /= (1 / _radius + 1 / other._radius);

            Vector2 impulse = impulseScalar * normal;
            // Применяем импульс к направлениям объектов
            _direction += impulse / _radius;
            other._direction -= impulse / other._radius;

            // Обеспечиваем небольшое раздвижение объектов, чтобы избежать застревания
            Vector2 separation = normal * (_radius + other._radius - Vector2.Distance(_position, other._position)) / 2f;
            _position += separation;
            other._position -= separation;
        }

        // Метод для попытки заражения другого человека при столкновении
        public void TryInfect(Human other)
        {
            // Проверяем, находится ли другой человек в радиусе заражения
            if (Vector2.Distance(_position, other._position) < infectionRadius)
            {
                // Проверяем, если текущий объект заражен, а другой человек здоров
                if ((this.State == HealthState.Infected || this.State == HealthState.Incubating) && other.State == HealthState.Healthy)
                {
                    // Генерируем случайное число и сравниваем его с вероятностью заражения
                    if (random.NextDouble() < infectionChance)
                    {
                        // Если вероятность сработала, заражаем другого человека
                        other.State = HealthState.Incubating;
                        other.infectionTimer = 0f; // Сброс таймера заражения для нового зараженного
                    }
                }

                // Проверяем, если текущий объект здоров, а другой человек заражен
                else if (this.State == HealthState.Healthy && (other.State == HealthState.Infected || other.State == HealthState.Incubating))
                {
                    // Генерируем случайное число и сравниваем его с вероятностью заражения
                    if (random.NextDouble() < infectionChance)
                    {
                        // Если вероятность сработала, заражаем текущего человека
                        this.State = HealthState.Incubating;
                        this.infectionTimer = 0f; // Сброс таймера заражения для нового зараженного
                    }
                }
            }
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
            changeDirectionTimer = 0f; // Сбрасываем таймер до 0
                                       // Устанавливаем случайный интервал до следующего изменения направления, от 1 до 4 секунд
            timeUntilNextChange = (float)(random.NextDouble() * 3) + 1;
        }

        // Обновление таймера для изменения направления
        private void UpdateDirectionTimer(GameTime gameTime)
        {
            // Увеличиваем таймер на прошедшее время с момента последнего обновления
            changeDirectionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Проверяем, если таймер достиг или превысил установленный интервал
            if (changeDirectionTimer >= timeUntilNextChange)
            {
                // Устанавливаем новое случайное направление
                _direction = GetRandomDirection();
                // Сбрасываем таймер и устанавливаем новый интервал
                ResetChangeDirectionTimer();
            }
        }

        // Проверка столкновения с границами экрана и изменение направления при необходимости
        private void CheckScreenCollision()
        {
            int offsetX = 10;
            int offsetY = 90;

            // Проверяем столкновение с левой или правой границей экрана
            if (_position.X - _radius <= offsetX || _position.X + _radius >= _screenWidth + offsetX)
            {
                // Меняем направление движения по оси X на противоположное
                _direction.X = -_direction.X;
                // Ограничиваем позицию так, чтобы объект оставался в пределах экрана
                _position.X = MathHelper.Clamp(_position.X, _radius + offsetX, _screenWidth + offsetX - _radius);
            }

            // Проверяем столкновение с верхней или нижней границей экрана
            if (_position.Y - _radius <= offsetY || _position.Y + _radius >= _screenHeight + offsetY)
            {
                // Меняем направление движения по оси Y на противоположное
                _direction.Y = -_direction.Y;
                // Ограничиваем позицию так, чтобы объект оставался в пределах экрана
                _position.Y = MathHelper.Clamp(_position.Y, _radius + offsetY, _screenHeight + offsetY - _radius);
            }
        }

        // Обновление таймера заражения и проверка выздоровления или смерти
        private void UpdateInfectionTimer(GameTime gameTime)
        {
            // Проверяем, если текущий объект находится в состоянии "инфицирован"
            if (State == HealthState.Incubating || State == HealthState.Infected)
            {
                // Увеличиваем таймер заражения на время, прошедшее с момента последнего обновления
                infectionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                // Увеличиваем таймер проверки на смерть
                deathCheckTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Проверка перехода состояния из "инкубационный период" в "инфицирован"
                if (State == HealthState.Incubating && infectionTimer >= incubationTime)
                {
                    // Если состояние инкубационное и таймер достиг времени инкубации, меняем состояние на "инфицирован"
                    State = HealthState.Infected;
                    deathCheckTimer = 0f; // Сброс таймера проверки на смерть при переходе в состояние инфицирования
                }

                // Проверяем, если таймер проверки на смерть достиг интервала проверки
                if (State == HealthState.Infected && deathCheckTimer >= deathCheckInterval)
                {
                    // Сбрасываем таймер проверки на смерть
                    deathCheckTimer = 0f;
                    // Проверяем вероятность смерти
                    if (random.NextDouble() < deathChance)
                    {
                        // Если вероятность сработала, изменяем состояние на "мертвый"
                        State = HealthState.Dead;
                        _direction = Vector2.Zero; // Остановка движения при смерти
                        speed = 0; // Остановка движения при смерти
                    }
                }

                // Проверяем, если таймер заражения достиг времени выздоровления
                if (State == HealthState.Infected && infectionTimer >= incubationTime + recoveryTime)
                {
                    // Изменяем состояние на "выздоровевший"
                    State = HealthState.Recovered;
                }
            }
        }

        // Получение цвета на основе состояния здоровья
        private Color GetHealthColor()
        {
            switch (State)
            {
                case HealthState.Healthy:
                    return Color.Green; // Здоровый - зеленый
                case HealthState.Incubating:
                    return Color.Pink; // Инкубационный период - розовый
                case HealthState.Infected:
                    return Color.Red; // Инфицированный - красный
                case HealthState.Recovered:
                    return Color.Blue; // Выздоровевший - синий
                case HealthState.Dead:
                    return Color.Black; // Мертвый - черный
                default:
                    return Color.White; // Неизвестное состояние - белый
            }
        }

        // Метод для создания текстуры круга (опционально)
        private Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int diameter)
        {
            // Создание новой текстуры с заданным диаметром
            Texture2D texture = new Texture2D(graphicsDevice, diameter, diameter);
            // Массив данных цветов для текстуры
            Color[] colorData = new Color[diameter * diameter];
            // Радиус круга
            float radius = diameter / 2f;
            // Квадрат радиуса для оптимизации вычислений
            float radiussq = radius * radius;

            // Заполнение данных цвета
            for (int x = 0; x < diameter; x++)
            {
                for (int y = 0; y < diameter; y++)
                {
                    // Индекс текущего пикселя в массиве цветов
                    int index = x * diameter + y;
                    // Позиция текущего пикселя относительно центра круга
                    Vector2 pos = new Vector2(x - radius, y - radius);
                    // Проверка, находится ли текущий пиксель внутри круга
                    if (pos.LengthSquared() <= radiussq)
                    {
                        colorData[index] = Color.White; // Пиксель внутри круга окрашивается в белый цвет
                    }
                    else
                    {
                        colorData[index] = Color.Transparent; // Пиксель вне круга делается прозрачным
                    }
                }
            }

            // Установка данных цветов в текстуру
            texture.SetData(colorData);
            return texture; // Возвращаем созданную текстуру
        }
    }
}
