using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SimulatorEpidemic
{
    public class MainMenu
    {
        private Texture2D _backgroundTexture; // Текстура фона главного меню
        private Rectangle _buttonRectangle;  // Прямоугольник, представляющий кнопку

        // Конструктор главного меню
        public MainMenu(Texture2D backgroundTexture)
        {
            _backgroundTexture = backgroundTexture;

            // Задаем координаты и размеры кнопки
            int buttonWidth = 154;
            int buttonHeight = 43;
            int buttonX = 563; // Координата X кнопки на экране
            int buttonY = 613; // Координата Y кнопки на экране
            _buttonRectangle = new Rectangle(buttonX, buttonY, buttonWidth, buttonHeight);
        }

        // Метод обновления состояния главного меню
        public bool Update(GameTime gameTime)
        {
            // Получаем состояние мыши
            MouseState mouseState = Mouse.GetState();

            // Проверяем, была ли нажата левая кнопка мыши и находится ли курсор на кнопке
            if (mouseState.LeftButton == ButtonState.Pressed &&
                _buttonRectangle.Contains(mouseState.Position))
            {
                return true; // Возвращаем true, если кнопка была нажата
            }

            return false; // Возвращаем false, если кнопка не была нажата
        }

        // Метод отрисовки главного меню
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            // Отрисовка фона
            spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), Color.White);
        }
    }
}
