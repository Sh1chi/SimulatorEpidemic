using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SimulatorEpidemic
{
    // Интерфейс IScreen определяет методы, которые должны быть реализованы любым классом, представляющим экран в игре.
    public interface IScreen
    {
        // Метод Initialize предназначен для инициализации экрана.
        void Initialize();

        // Метод LoadContent предназначен для загрузки контента, необходимого для отображения экрана.
        void LoadContent();

        // Метод Update предназначен для обновления логики экрана, принимая в качестве параметра объект GameTime.
        void Update(GameTime gameTime);

        // Метод Draw предназначен для отрисовки экрана, принимая в качестве параметров объект GameTime и объект SpriteBatch.
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
