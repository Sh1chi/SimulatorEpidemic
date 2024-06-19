using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SimulatorEpidemic
{
    public class GifAnimation
    {
        // Список кадров анимации в виде Texture2D
        private List<Texture2D> frames;
        // Список задержек между кадрами в миллисекундах
        private List<int> frameDelays;
        // Индекс текущего кадра
        private int currentFrame;
        // Время, прошедшее с момента последнего обновления кадра
        private double timeElapsed;

        // Конструктор принимает GraphicsDevice и путь к GIF файлу
        public GifAnimation(GraphicsDevice graphicsDevice, string filePath)
        {
            frames = new List<Texture2D>();
            frameDelays = new List<int>();

            // Загружаем GIF изображение
            using (var gifImage = System.Drawing.Image.FromFile(filePath))
            {
                // Получаем список кадров в GIF
                var dimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
                int frameCount = gifImage.GetFrameCount(dimension);

                // Перебираем все кадры в GIF
                for (int i = 0; i < frameCount; i++)
                {
                    gifImage.SelectActiveFrame(dimension, i);

                    // Создаем новый Bitmap для каждого кадра
                    var frame = new Bitmap(gifImage.Width, gifImage.Height);
                    using (var graphics = Graphics.FromImage(frame))
                    {
                        graphics.DrawImage(gifImage, System.Drawing.Point.Empty);
                    }

                    // Преобразуем Bitmap в Texture2D и добавляем в список кадров
                    frames.Add(Texture2DFromBitmap(graphicsDevice, frame));
                    // Добавляем задержку кадра в список задержек (в сотых долях секунды)
                    frameDelays.Add(BitConverter.ToInt32(gifImage.GetPropertyItem(0x5100).Value, i * 4) * 10);
                }
            }

            currentFrame = 0;
            timeElapsed = 0;
        }

        // Метод для преобразования Bitmap в Texture2D
        private Texture2D Texture2DFromBitmap(GraphicsDevice graphicsDevice, Bitmap bitmap)
        {
            var texture = new Texture2D(graphicsDevice, bitmap.Width, bitmap.Height);
            var data = new byte[4 * bitmap.Width * bitmap.Height];
            var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // Копируем данные из Bitmap в массив байтов
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, data, 0, data.Length);
            texture.SetData(data);

            bitmap.UnlockBits(bitmapData);
            return texture;
        }

        // Метод для обновления текущего кадра анимации
        public void Update(GameTime gameTime)
        {
            // Увеличиваем время, прошедшее с последнего обновления кадра
            timeElapsed += gameTime.ElapsedGameTime.TotalMilliseconds;

            // Если прошло достаточно времени, переключаемся на следующий кадр
            if (timeElapsed >= frameDelays[currentFrame])
            {
                currentFrame = (currentFrame + 1) % frames.Count;
                timeElapsed = 0;
            }
        }

        // Метод для отрисовки текущего кадра анимации
        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(frames[currentFrame], position, Microsoft.Xna.Framework.Color.White);
        }
    }
}
