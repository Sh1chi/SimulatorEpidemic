using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SimulatorEpidemic
{
    // Основной класс игры, наследующийся от Game
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics; // Управляет графическими устройствами
        private SpriteBatch _spriteBatch; // Отвечает за пакетную отрисовку спрайтов
        private GameStateManager _gameStateManager; // Менеджер состояний игры

        // Конструктор игры
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;  // Устанавливаем ширину окна
            _graphics.PreferredBackBufferHeight = 720; // Устанавливаем высоту окна
            Content.RootDirectory = "Content"; // Директория для контента
            IsMouseVisible = true; // Делает курсор мыши видимым
        }

        // Инициализация игры
        protected override void Initialize()
        {
            _gameStateManager = GameStateManager.Instance; // Получаем экземпляр менеджера состояний игры
            _gameStateManager.AddScreen("MainMenu", new MainMenu(Content)); // Добавляем главный экран
            _gameStateManager.AddScreen("EpidemicSimulator", new EpidemicSimulator(Content, GraphicsDevice)); // Добавляем экран симулятора эпидемии
            _gameStateManager.ChangeScreen("MainMenu"); // Устанавливаем начальный экран на главное меню

            _gameStateManager.Initialize(); // Инициализация менеджера состояний
            base.Initialize(); // Вызов базового метода Initialize
        }

        // Загрузка контента игры
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice); // Создаем объект SpriteBatch

            _gameStateManager.LoadContent(); // Загрузка контента для менеджера состояний
        }

        // Обновление логики игры
        protected override void Update(GameTime gameTime)
        {
            // Проверка нажатия кнопки "Back" на геймпаде или клавиши "Escape" на клавиатуре для выхода из игры
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _gameStateManager.Update(gameTime); // Обновление состояния игры через менеджера состояний

            base.Update(gameTime); // Вызов базового метода Update
        }

        // Отрисовка игры
        protected override void Draw(GameTime gameTime)
        {
            // Очистка экрана цветом "CornflowerBlue"
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Начало пакетной отрисовки
            _spriteBatch.Begin();

            // Отрисовка в зависимости от текущего состояния игры
            _gameStateManager.Draw(gameTime, _spriteBatch);

            // Завершение пакетной отрисовки
            _spriteBatch.End();

            // Вызов базового метода Draw
            base.Draw(gameTime);
        }
    }
}
