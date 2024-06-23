using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

public class Slider
{
    private Texture2D sliderTexture; // Текстура для отображения линии ползунка
    private Texture2D knobTexture; // Текстура для отображения ручки ползунка
    private Rectangle sliderRectangle; // Прямоугольник, определяющий область ползунка
    private Rectangle knobRectangle; // Прямоугольник, определяющий область ручки
    private Rectangle filledRectangle; // Прямоугольник, определяющий закрашенную часть ползунка
    private Vector2 position; // Позиция ползунка на экране
    private bool isDragging; // Флаг, указывающий, перемещается ли ручка в данный момент
    private float minValue; // Минимальное значение ползунка
    private float maxValue; // Максимальное значение ползунка
    private float currentValue; // Текущее значение ползунка
    private int knobWidth; // Ширина ручки
    private int knobHeight; // Высота ручки
    private SpriteFont font; // Шрифт для отображения текста
    private string sliderName; // Название ползунка

    // Конструктор для инициализации ползунка
    public Slider(Texture2D sliderTexture, Texture2D knobTexture, Vector2 position, float minValue, float maxValue, float initialValue, SpriteFont font, string sliderName)
    {
        this.sliderTexture = sliderTexture;
        this.knobTexture = knobTexture;
        this.position = position;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.currentValue = initialValue;
        this.sliderRectangle = new Rectangle((int)position.X, (int)position.Y, sliderTexture.Width, sliderTexture.Height);
        this.knobWidth = knobTexture.Width;
        this.knobHeight = knobTexture.Height;
        this.font = font;
        this.sliderName = sliderName;
        UpdateKnobPosition(); // Обновляем позицию ручки и закрашенной части
    }

    // Свойство для получения текущего значения ползунка
    public float Value => currentValue;

    // Метод для обновления позиции ручки и закрашенной части
    private void UpdateKnobPosition()
    {
        // Вычисляем относительное положение ручки (от 0 до 1)
        float relativePosition = (currentValue - minValue) / (maxValue - minValue);

        // Обновляем позицию прямоугольника ручки
        knobRectangle = new Rectangle(
            (int)(sliderRectangle.X + relativePosition * (sliderRectangle.Width - knobWidth)),
            (int)(sliderRectangle.Y + sliderRectangle.Height / 2 - knobHeight / 2),
            knobWidth,
            knobHeight
        );

        // Обновляем позицию и размер закрашенной части ползунка
        filledRectangle = new Rectangle(
            sliderRectangle.X,
            sliderRectangle.Y,
            knobRectangle.X - sliderRectangle.X,
            sliderRectangle.Height
        );
    }

    // Метод для обновления состояния ползунка на основе ввода мыши
    public void Update(GameTime gameTime)
    {
        MouseState mouseState = Mouse.GetState();

        // Проверяем, нажата ли левая кнопка мыши и находится ли указатель на ручке
        if (mouseState.LeftButton == ButtonState.Pressed && knobRectangle.Contains(mouseState.Position))
        {
            isDragging = true;
        }

        // Если ручка перетаскивается
        if (isDragging)
        {
            int mouseX = mouseState.X;
            // Ограничиваем перемещение ручки в пределах ползунка
            mouseX = Math.Clamp(mouseX, sliderRectangle.X, sliderRectangle.X + sliderRectangle.Width - knobWidth);
            // Вычисляем новое относительное положение ручки
            float relativePosition = (float)(mouseX - sliderRectangle.X) / (sliderRectangle.Width - knobWidth);
            // Обновляем текущее значение ползунка
            currentValue = minValue + relativePosition * (maxValue - minValue);
            // Обновляем позицию ручки и закрашенной части
            UpdateKnobPosition();
        }

        // Если левая кнопка мыши отпущена, прекращаем перетаскивание
        if (mouseState.LeftButton == ButtonState.Released)
        {
            isDragging = false;
        }
    }

    // Метод для отрисовки ползунка
    public void Draw(SpriteBatch spriteBatch)
    {
        // Отрисовка закрашенной части ползунка
        spriteBatch.Draw(sliderTexture, filledRectangle, Color.Red);
        // Отрисовка незакрашенной части ползунка
        spriteBatch.Draw(sliderTexture, new Rectangle(filledRectangle.Right, sliderRectangle.Y, sliderRectangle.Width - filledRectangle.Width, sliderRectangle.Height), Color.White);
        // Отрисовка ручки ползунка
        spriteBatch.Draw(knobTexture, knobRectangle, Color.Red);

        // Отрисовка текущего значения под ручкой
        string valueText = currentValue.ToString("F2"); // Преобразование текущего значения в строку с двумя знаками после запятой
        Vector2 textSize = font.MeasureString(valueText); // Измерение размера текста
        Vector2 textPosition = new Vector2(knobRectangle.X + knobRectangle.Width / 2 - textSize.X / 2, knobRectangle.Y + knobRectangle.Height); // Позиция текста под ручкой
        spriteBatch.DrawString(font, valueText, textPosition, Color.White); // Отрисовка текста

        // Отрисовка названия ползунка над ползунком
        Vector2 nameSize = font.MeasureString(sliderName); // Измерение размера текста названия
        Vector2 namePosition = new Vector2(sliderRectangle.X + sliderRectangle.Width / 2 - nameSize.X / 2, sliderRectangle.Y - nameSize.Y - 10); // Позиция текста названия над ползунком
        spriteBatch.DrawString(font, sliderName, namePosition, Color.White); // Отрисовка текста названия
    }
}
