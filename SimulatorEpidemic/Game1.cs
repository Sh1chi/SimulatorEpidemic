using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SimulatorEpidemic
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Human _human;
        private Texture2D _circleTexture;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Создание текстуры круга 20x20
            _circleTexture = CreateCircleTexture(10);

            // Инициализация человека
            _human = new Human(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        }

        private Texture2D CreateCircleTexture(int radius)
        {
            int diameter = radius * 2;
            Texture2D texture = new Texture2D(GraphicsDevice, diameter, diameter);
            Color[] colorData = new Color[diameter * diameter];

            float radiussq = radius * radius;

            for (int x = 0; x < diameter; x++)
            {
                for (int y = 0; y < diameter; y++)
                {
                    int index = x * diameter + y;
                    Vector2 pos = new Vector2(x - radius, y - radius);
                    if (pos.LengthSquared() <= radiussq)
                    {
                        colorData[index] = Color.White;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Логика обновления

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _human.Draw(_spriteBatch, _circleTexture);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
