using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SimulatorEpidemic;
using System.Collections.Generic;

public class GameStateManager
{
    // Словарь для хранения экранов, где ключ - это имя экрана, а значение - сам экран.
    private Dictionary<string, IScreen> screens;

    // Переменная для хранения текущего активного экрана.
    private IScreen currentScreen;

    // Переменная для хранения экрана, на который будет переключение.
    private IScreen nextScreen;

    // Статическая переменная для хранения единственного экземпляра класса GameStateManager.
    private static GameStateManager _instance;

    // Свойство для доступа к единственному экземпляру класса GameStateManager (реализация паттерна Singleton).
    public static GameStateManager Instance
    {
        get
        {
            // Если экземпляр еще не создан, создаем его.
            if (_instance == null)
            {
                _instance = new GameStateManager();
            }
            return _instance;
        }
    }

    // Конструктор по умолчанию, инициализирующий словарь экранов.
    public GameStateManager()
    {
        screens = new Dictionary<string, IScreen>();
    }

    // Метод для добавления экрана в менеджер экранов.
    public void AddScreen(string name, IScreen screen)
    {
        if (!screens.ContainsKey(name))
        {
            screens[name] = screen;
        }
    }

    // Метод для удаления экрана из менеджера экранов.
    public void RemoveScreen(string name)
    {
        if (screens.ContainsKey(name))
        {
            screens.Remove(name);
        }
    }

    // Метод для смены текущего экрана на новый.
    public void ChangeScreen(string name)
    {
        if (screens.ContainsKey(name))
        {
            nextScreen = screens[name];
        }
    }

    // Метод для инициализации всех экранов в менеджере.
    public void Initialize()
    {
        foreach (var screen in screens.Values)
        {
            screen.Initialize();
        }
    }

    // Метод для загрузки контента всех экранов в менеджере.
    public void LoadContent()
    {
        foreach (var screen in screens.Values)
        {
            screen.LoadContent();
        }
    }

    // Метод для обновления логики текущего экрана.
    public void Update(GameTime gameTime)
    {
        // Если был выбран новый экран, переключаемся на него.
        if (nextScreen != null)
        {
            currentScreen = nextScreen;
            nextScreen = null;
        }

        // Обновляем текущий экран.
        currentScreen?.Update(gameTime);
    }

    // Метод для отрисовки текущего экрана.
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        // Отрисовываем текущий экран.
        currentScreen?.Draw(gameTime, spriteBatch);
    }
}
