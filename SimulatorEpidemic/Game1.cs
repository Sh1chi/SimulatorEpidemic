using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SimulatorEpidemic
{
    // Основной класс игры, наследующийся от Game
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics; // Управляет графическими устройствами
        private SpriteBatch _spriteBatch; // Отвечает за пакетную отрисовку спрайтов
        private Human _human; // Объект человека в игре
        private Texture2D _humanTexture; // Текстура для отображения человека
        private Random random; // Объект для генерации случайных чисел

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

            // Инициализация объекта человека с размерами экрана и радиусом, основанным на ширине текстуры
            _human = new Human(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, _humanTexture.Width / 2);
        }

        // Обновление логики игры
        protected override void Update(GameTime gameTime)
        {
            // Проверка нажатия кнопки "Back" на геймпаде или клавиши "Escape" на клавиатуре для выхода из игры
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Обновление состояния человека
            _human.Update(gameTime);

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
            _human.Draw(_spriteBatch, _humanTexture);
            // Завершение пакетной отрисовки
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
