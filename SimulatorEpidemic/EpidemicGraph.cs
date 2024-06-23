using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SimulatorEpidemic
{
    internal class EpidemicGraph
    {
        // Текстура для рисования пикселей
        private Texture2D _pixel;
        // Списки точек данных для каждого состояния здоровья
        private List<Vector2> _healthyPoints;
        private List<Vector2> _infectedPoints;
        private List<Vector2> _deadPoints;
        // Время для оси X
        private float _time;
        // Позиция и размер графика
        private Vector2 _position;
        private Vector2 _size;
        // Максимальное количество точек данных
        private int _maxDataPoints;
        // Общее количество людей
        private int _totalHumans;

        // Конструктор инициализирует график
        public EpidemicGraph(GraphicsDevice graphicsDevice, Vector2 position, Vector2 size, int totalHumans)
        {
            // Создаем текстуру пикселя
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            // Инициализация списков точек данных
            _healthyPoints = new List<Vector2>();
            _infectedPoints = new List<Vector2>();
            _deadPoints = new List<Vector2>();
            _time = 0f;
            _position = position;
            _size = size;
            _maxDataPoints = (int)size.X;
            _totalHumans = totalHumans;
        }

        // Метод для добавления новых точек данных
        public void AddDataPoints(int healthy, int infected, int dead)
        {
            _time += 1f; // Увеличиваем время для оси X

            // Добавляем новые точки данных для каждого состояния
            _healthyPoints.Add(new Vector2(_time, healthy));
            _infectedPoints.Add(new Vector2(_time, infected));
            _deadPoints.Add(new Vector2(_time, dead));

            // Удаляем старые точки данных, если их количество превышает максимальное
            if (_healthyPoints.Count > _maxDataPoints)
            {
                _healthyPoints.RemoveAt(0);
                _infectedPoints.RemoveAt(0);
                _deadPoints.RemoveAt(0);
                // Обновляем X координаты оставшихся точек
                for (int i = 0; i < _healthyPoints.Count; i++)
                {
                    _healthyPoints[i] = new Vector2(i, _healthyPoints[i].Y);
                    _infectedPoints[i] = new Vector2(i, _infectedPoints[i].Y);
                    _deadPoints[i] = new Vector2(i, _deadPoints[i].Y);
                }
            }
        }

        // Метод для отрисовки графика
        public void Draw(SpriteBatch spriteBatch)
        {
            // Отрисовка фона графика
            spriteBatch.Draw(_pixel, new Rectangle((int)_position.X, (int)_position.Y, (int)_size.X, (int)_size.Y), Color.Black * 0.8f);

            // Проверка, есть ли достаточно данных для рисования
            if (_healthyPoints.Count < 2)
                return;

            // Отрисовка областей для каждого состояния здоровья
            DrawArea(spriteBatch, _healthyPoints, Color.Green);
            DrawArea(spriteBatch, _infectedPoints, Color.Red);
            DrawArea(spriteBatch, _deadPoints, Color.Gray);
        }

        // Метод для отрисовки области графика
        private void DrawArea(SpriteBatch spriteBatch, List<Vector2> points, Color color)
        {
            for (int i = 1; i < points.Count; i++)
            {
                // Вычисляем начальную и конечную точки линии
                Vector2 start = _position + new Vector2(points[i - 1].X, _size.Y - (points[i - 1].Y / _totalHumans * _size.Y));
                Vector2 end = _position + new Vector2(points[i].X, _size.Y - (points[i].Y / _totalHumans * _size.Y));
                // Отрисовываем линию
                DrawLine(spriteBatch, start, end, color);
            }
        }

        // Метод для отрисовки линии
        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
        {
            float distance = Vector2.Distance(start, end);
            float angle = (float)System.Math.Atan2(end.Y - start.Y, end.X - start.X);

            // Отрисовываем линию с использованием текстуры пикселя
            spriteBatch.Draw(_pixel, new Rectangle((int)start.X, (int)start.Y, (int)distance, 1), null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
