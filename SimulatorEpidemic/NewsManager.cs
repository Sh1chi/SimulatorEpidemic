using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatorEpidemic
{
    public class NewsManager
    {
        private string[] newsArray; // Массив новостей
        private int currentNewsIndex; // Индекс текущей новости
        private double timeSinceLastUpdate; // Время, прошедшее с последнего обновления новости
        private double updateInterval; // Интервал обновления новостей в секундах
        private Random random; // Объект для генерации случайных чисел

        private string currentNews; // Текущая новость
        private int charIndex; // Индекс текущего символа в новости
        private double charInterval; // Интервал между появлением символов в секундах
        private double timeSinceLastChar; // Время, прошедшее с последнего появления символа
        private bool isNewsFullyDisplayed; // Флаг для проверки, полностью ли отображена новость

        private SoundEffect typingSound; // Звук набора текста
        private SoundEffectInstance typingSoundInstance; // Экземпляр звука набора текста

        public NewsManager(string[] initialNews, double intervalInSeconds, double charIntervalInSeconds, SoundEffect typingSound)
        {
            newsArray = initialNews; // Инициализация массива новостей
            currentNewsIndex = 0; // Инициализация индекса текущей новости
            timeSinceLastUpdate = 0; // Инициализация времени с последнего обновления
            updateInterval = intervalInSeconds; // Установка интервала обновления новостей
            random = new Random(); // Инициализация объекта Random

            currentNews = newsArray[currentNewsIndex]; // Установка текущей новости
            charIndex = 0; // Инициализация индекса текущего символа
            charInterval = charIntervalInSeconds; // Установка интервала между появлением символов
            timeSinceLastChar = 0; // Инициализация времени с последнего появления символа
            isNewsFullyDisplayed = false; // Инициализация флага

            this.typingSound = typingSound; // Инициализация звука набора текста
            typingSoundInstance = typingSound.CreateInstance(); // Создание экземпляра звука
        }


        public string GetCurrentNews(GameTime gameTime)
        {
            if (!isNewsFullyDisplayed)
            {
                // Обновление времени с последнего появления символа
                timeSinceLastChar += gameTime.ElapsedGameTime.TotalSeconds;

                // Проверка, нужно ли добавить новый символ
                if (timeSinceLastChar >= charInterval && charIndex < currentNews.Length)
                {
                    timeSinceLastChar = 0;
                    charIndex++; // Увеличение индекса текущего символа
                    if (typingSoundInstance.State != SoundState.Playing)
                    {
                        typingSoundInstance.Play(); // Воспроизведение звука набора текста
                    }
                }

                // Проверка, полностью ли отображена новость
                if (charIndex >= currentNews.Length)
                {
                    isNewsFullyDisplayed = true;
                    timeSinceLastUpdate = 0; // Сброс времени для следующей новости
                    typingSoundInstance.Stop(); // Остановка звука набора текста
                }
            }
            else
            {
                // Обновление времени с последнего обновления новости
                timeSinceLastUpdate += gameTime.ElapsedGameTime.TotalSeconds;

                // Проверка, нужно ли обновить новость
                if (timeSinceLastUpdate >= updateInterval)
                {
                    timeSinceLastUpdate = 0;
                    currentNewsIndex = random.Next(newsArray.Length); // Случайный выбор новой новости
                    currentNews = newsArray[currentNewsIndex]; // Установка новой текущей новости
                    charIndex = 0; // Сброс индекса символов
                    isNewsFullyDisplayed = false; // Сброс флага
                }
            }

            // Возврат текущей части новости
            return currentNews.Substring(0, charIndex);
        }

        // Метод для переноса текста на новую строку, если он превышает заданную ширину
        public string WrapText(string text, SpriteFont font, float maxLineWidth)
        {
            string[] lines = text.Split('\n'); // Разделяем текст на строки по символу новой строки
            StringBuilder wrappedText = new StringBuilder();

            foreach (var line in lines)
            {
                string[] words = line.Split(' '); // Разделяем строки на слова
                float lineWidth = 0;
                float spaceWidth = font.MeasureString(" ").X;

                foreach (var word in words)
                {
                    Vector2 size = font.MeasureString(word);
                    if (lineWidth + size.X < maxLineWidth)
                    {
                        wrappedText.Append(word + " ");
                        lineWidth += size.X + spaceWidth;
                    }
                    else
                    {
                        wrappedText.Append("\n" + word + " ");
                        lineWidth = size.X + spaceWidth;
                    }
                }
                wrappedText.Append("\n"); // Добавляем перенос строки после каждой исходной строки
            }

            return wrappedText.ToString();
        }
    }
}
