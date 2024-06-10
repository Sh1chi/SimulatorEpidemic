using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SimulatorEpidemic
{
    public class MainMenu
    {
        private Texture2D _backgroundTexture;
        private Rectangle _buttonRectangle;

        public MainMenu(Texture2D backgroundTexture)
        {
            _backgroundTexture = backgroundTexture;

            // Координаты и размеры кнопки
            int buttonWidth = 154;
            int buttonHeight = 43;
            int buttonX = 563; // Координата X кнопки на экране
            int buttonY = 613; // Координата Y кнопки на экране
            _buttonRectangle = new Rectangle(buttonX, buttonY, buttonWidth, buttonHeight);
        }

        public bool Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed &&
                _buttonRectangle.Contains(mouseState.Position))
            {
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            // Отрисовка фона
            spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), Color.White);
        }
    }
}
