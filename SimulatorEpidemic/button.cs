using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

public class Button
{
    private Texture2D texture; // Текстура кнопки
    private Rectangle rectangle; // Прямоугольник кнопки
    private MouseState previousMouseState; // Предыдущее состояние мыши
    private SoundEffect clickSound; // Звуковой эффект при нажатии кнопки

    public bool IsClicked { get; private set; } // Флаг, указывающий, была ли кнопка нажата
    public bool IsEnabled { get; set; } // Флаг, указывающий, доступна ли кнопка

    // Конструктор класса Button
    public Button(Texture2D texture, Rectangle rectangle, SoundEffect clickSound)
    {
        this.texture = texture;
        this.rectangle = rectangle;
        this.clickSound = clickSound;
        this.IsEnabled = true; // По умолчанию кнопка доступна
    }

    // Метод для обновления состояния кнопки
    public void Update(MouseState currentMouseState)
    {
        IsClicked = false; // Сбрасываем флаг нажатия
        if (IsEnabled && currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
        {
            if (rectangle.Contains(currentMouseState.Position)) // Проверяем, находится ли курсор в пределах кнопки
            {
                clickSound.Play(); // Воспроизводим звук нажатия
                IsClicked = true; // Устанавливаем флаг нажатия
            }
        }
        previousMouseState = currentMouseState; // Сохраняем текущее состояние мыши для следующего кадра
    }

    // Метод для отрисовки кнопки
    public void Draw(SpriteBatch spriteBatch)
    {
        Color color = IsEnabled ? Color.White : Color.Gray; // Если кнопка недоступна, рисуем её серой
        spriteBatch.Draw(texture, rectangle, color); // Отрисовываем кнопку
    }
}
