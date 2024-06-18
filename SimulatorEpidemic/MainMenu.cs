using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SimulatorEpidemic
{
    // Класс главного меню, реализующий интерфейс IScreen
    public class MainMenu : IScreen
    {
        private Texture2D _backgroundMenuTexture; // Текстура фона главного меню
        private Texture2D _startButtonTexture; // Текстура кнопки старт
        private Rectangle _buttonRectangle; // Прямоугольник, представляющий кнопку
        private ContentManager _content; // Менеджер контента для загрузки ресурсов

        // Конструктор главного меню
        public MainMenu(ContentManager content)
        {
            _content = content;
        }

        // Метод инициализации
        public void Initialize(){ }

        // Метод загрузки контента
        public void LoadContent()
        {
            // Загрузка текстуры фона главного меню
            _backgroundMenuTexture = _content.Load<Texture2D>("backgroundMenu");
            
            // Загрузка текстуры кнопки старт
            _startButtonTexture = _content.Load<Texture2D>("button_start");

            // Задаем координаты и размеры кнопки
            _buttonRectangle = new Rectangle(835, 890, _startButtonTexture.Width, _startButtonTexture.Height);
        }

        // Метод обновления состояния главного меню
        public void Update(GameTime gameTime)
        {
            // Получаем состояние мыши
            MouseState mouseState = Mouse.GetState();

            // Проверяем, была ли нажата левая кнопка мыши и находится ли курсор на кнопке
            if (mouseState.LeftButton == ButtonState.Pressed &&
                _buttonRectangle.Contains(mouseState.Position))
            {
                // Изменение экрана на симулятор эпидемии
                GameStateManager.Instance.ChangeScreen("EpidemicSimulator");
            }
        }

        // Метод отрисовки главного меню
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Отрисовка фона главного меню
            spriteBatch.Draw(_backgroundMenuTexture, new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), Color.White);

            // Отрисовка кнопки старт
            spriteBatch.Draw(_startButtonTexture, _buttonRectangle, Color.White);
        }
    }
}
